using System.Buffers;
using System.Text.RegularExpressions;
using TheOmenDen.Shared.Extensions;

namespace TheOmenDen.CrowsAgainstHumility.Services.Authentication;

/// <summary>
/// This class generates a (pseudo)hashed game code to allow players to securely connect to a game.
/// </summary>
internal sealed class GameCodeGenerator: IDisposable, IAsyncDisposable
{
    #region Constants
    public const string DEFAULT_ALPHABET_LOWER = "abcdefghijklmnopqrstuvwxyz";
    public const string DEFAULT_ALPHABET_UPPER = $"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string DEFAULT_ALPHABET = $"{DEFAULT_ALPHABET_LOWER}{DEFAULT_ALPHABET_UPPER}";
    public const string DEFAULT_SEPARATORS = "cfhistuCFHISTU";
    public const int MINIMUM_ALPHABET_LENGTH = 16;

    private const double _separator_Divisions = 3.5;

    private const double _guard_Divisions = 12.0;

    private const int _maximum_Number_Size = 12;
    #endregion
    #region Private Fields
    private readonly char[] _alphabet;
    private readonly char[] _separators;
    private readonly char[] _guards;
    private readonly char[] _salt;
    private readonly int _minimumHashLength;

    private static readonly Lazy<Regex> _hexValidator = new(() => new Regex("^[0-9a-fA-F]+$", RegexOptions.Compiled));
    private static readonly Lazy<Regex> _hexSplitter = new(() => new Regex(@"[\w\W]{1,12}", RegexOptions.Compiled));
    private bool disposedValue;
    #endregion
    #region Constructors
    public GameCodeGenerator()
    : this(string.Empty, 0, DEFAULT_ALPHABET, DEFAULT_SEPARATORS)
    {
    }

    public GameCodeGenerator(String salt = "", Int32 minimumHashLength = 0, String alphabet = DEFAULT_ALPHABET, String separators = DEFAULT_SEPARATORS)
    {
        if (!StringBuilderPoolFactory<GameCodeGenerator>.Exists(nameof(GameCodeGenerator)))
        {
            StringBuilderPoolFactory<GameCodeGenerator>.Create(nameof(GameCodeGenerator));
        }

        if (salt == null)
        {
            throw new ArgumentNullException(nameof(salt));
        }

        if (String.IsNullOrWhiteSpace(alphabet))
        {
            throw new ArgumentNullException(nameof(alphabet));
        }

        if (minimumHashLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(minimumHashLength),
                @"Value must be greater than or equal to 0 (zero)");
        }

        if (String.IsNullOrWhiteSpace(separators))
        {
            throw new ArgumentNullException(nameof(separators));
        }

        _salt = salt.Trim().ToCharArray();

        _minimumHashLength = minimumHashLength;

        (_alphabet, _separators, _guards) = InitializeCharacterArrays(alphabet: alphabet, separators: separators, salt: _salt);
    }
    #endregion
    #region Private Methods
    private static (Char[] alphabetChars, Char[] separatorChars, Char[] guardChars) InitializeCharacterArrays(String alphabet, String separators, ReadOnlySpan<char> salt)
    {
        var (alphabetChars, separatorChars, guardChars) = (alphabet.ToCharArray().Distinct().ToArray(),
            separators.ToCharArray(), Array.Empty<Char>());

        if (alphabetChars.Length < MINIMUM_ALPHABET_LENGTH)
        {
            throw new ArgumentException(
                $@"Alphabet must contain at least {MINIMUM_ALPHABET_LENGTH: N0} unique characters",
                paramName: nameof(alphabet));
        }

        if (separators.Length > 0)
        {
            separatorChars = alphabetChars.Except(separatorChars).ToArray();
        }

        if (alphabetChars.Length > 0)
        {
            alphabetChars = alphabetChars.Except(separatorChars).ToArray();
        }

        if (alphabetChars.Length < (MINIMUM_ALPHABET_LENGTH - 6))
        {
            throw new ArgumentException(
                $@"Alphabet must contain at least {MINIMUM_ALPHABET_LENGTH: N0} unique characters that are also not present in .",
                paramName: nameof(alphabet));
        }

        ConsistentShuffle(separatorChars, separatorChars.Length, salt, salt.Length);

        if (separatorChars.Length == 0 || (float)alphabetChars.Length / separatorChars.Length > _separator_Divisions)
        {
            var separatorsLength = (int)Math.Ceiling((float)alphabetChars.Length / _separator_Divisions);

            if (separatorsLength == 1)
            {
                separatorsLength = 2;
            }

            if (separatorsLength > separatorChars.Length)
            {
                var differenceInLengths = separatorsLength - separatorChars.Length;

                separatorChars = separatorChars.Append(alphabetChars, 0, differenceInLengths);

                alphabetChars = alphabetChars.SubArray(differenceInLengths);
            }
            else
            {
                separatorChars = separatorChars.SubArray(0, separatorsLength);
            }
        }

        ConsistentShuffle(alphabetChars, alphabetChars.Length, salt, salt.Length);

        var guardCount = (int)Math.Ceiling((float)alphabetChars.Length / _guard_Divisions);

        if (alphabetChars.Length < 3)
        {
            guardChars = separatorChars.SubArray(index: 0, length: guardCount);
            separatorChars = separatorChars.SubArray(index: guardCount);
        }
        else
        {
            guardChars = alphabetChars.SubArray(index: 0, length: guardCount);
            alphabetChars = alphabetChars.SubArray(index: guardCount);
        }

        return (alphabetChars, separatorChars, guardChars);
    }

    private String GenerateHashFrom(ReadOnlySpan<Int64> numbers)
    {
        if (numbers.Length == 0 || numbers.Any(number => number < 0))
        {
            return String.Empty;
        }

        var numbersHashRepresentative = 0L;

        for (var i = 0; i < numbers.Length; i++)
        {
            numbersHashRepresentative += numbers[i] % (i + 100);
        }

        var builder = StringBuilderPoolFactory<GameCodeGenerator>.Get(nameof(GameCodeGenerator));

        var shuffleBuffer = Array.Empty<Char>();

        var alphabet = _alphabet.CopyPooled();

        var hashBuffer = ArrayPool<Char>.Shared.Rent(_maximum_Number_Size);

        try
        {
            var lottery = alphabet[numbersHashRepresentative % _alphabet.Length];

            builder.Append(lottery);

            shuffleBuffer = CreatePooledBuffer(_alphabet.Length, lottery);

            var startIndex = 1 + _salt.Length;

            var length = _alphabet.Length - startIndex;

            for (var i = startIndex; i < length; i++)
            {
                var number = numbers[i];

                if (length > 0)
                {
                    Array.Copy(alphabet, 0, shuffleBuffer, startIndex, length);
                }

                ConsistentShuffle(alphabet, _alphabet.Length, shuffleBuffer, _alphabet.Length);

                var hashLength = BuildReversedHash(number, alphabet, hashBuffer);

                for (var j = hashLength - 1; j > -1; j--)
                {
                    builder.Append(hashBuffer[j]);
                }

                if (i + 1 >= numbers.Length)
                {
                    continue;
                }

                number %= hashBuffer[hashLength - 1] + i;

                var separatorsIndex = number % _separators.Length;

                builder.Append(_separators[separatorsIndex]);
            }

            if (builder.Length < _minimumHashLength)
            {
                var guardIndex = (numbersHashRepresentative + builder[0]) % _guards.Length;
                var guard = _guards[guardIndex];

                builder.Insert(0, guard);

                if (builder.Length < _minimumHashLength)
                {
                    guardIndex = (numbersHashRepresentative + builder[2]) % _guards.Length;

                    guard = _guards[guardIndex];

                    builder.Append(guard);
                }
            }

            var halfLength = _alphabet.Length / 2;

            while (builder.Length < _minimumHashLength)
            {
                Array.Copy(alphabet, shuffleBuffer, _alphabet.Length);

                ConsistentShuffle(alphabet, _alphabet.Length, shuffleBuffer, _alphabet.Length);

                builder.Insert(0, alphabet, halfLength, _alphabet.Length - halfLength);
                builder.Append(alphabet, 0, halfLength);

                var excessCharacterCount = builder.Length - _minimumHashLength;

                if (excessCharacterCount <= 0)
                {
                    continue;
                }

                builder.Remove(0, excessCharacterCount / 2);
                builder.Remove(_minimumHashLength, builder.Length - _minimumHashLength);
            }
        }
        finally
        {
            alphabet.ReturnArrayToPool();
            shuffleBuffer.ReturnArrayToPool();
            hashBuffer.ReturnArrayToPool();
        }

        var result = builder.ToString();

        return result;
    }

    private Int32 BuildReversedHash(Int64 input, ReadOnlySpan<Char> alphabet, Char[] hashBuffer)
    {
        var length = 0;

        do
        {
            var index = (int)(input % _alphabet.Length);

            hashBuffer[length] = alphabet[index];

            length += 1;

            input /= _alphabet.Length;
        } while (input > 0);

        return length;
    }

    private Int64 Unhash(String input, ReadOnlySpan<Char> alphabet)
    {
        var number = 0L;

        for (var i = 0; i < input.Length; i++)
        {
            var position = alphabet.IndexOf(input[i]);

            number = (number * _alphabet.Length) + position;
        }

        return number;
    }

    private Int64[] GetNumbersFrom(String hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            return Array.Empty<Int64>();
        }

        var hashArray = hash.Split(_guards, StringSplitOptions.RemoveEmptyEntries);

        if (hashArray.Length == 0)
        {
            return Array.Empty<Int64>();
        }


        var patternIterator = (hashArray.Length is 3 or 2) ? 1 : 0;

        var hashBreakdown = hashArray[patternIterator];

        var lottery = hashBreakdown[0];

        //default(char) == '\0'
        if (lottery.Equals('\0'))
        {
            return Array.Empty<Int64>();
        }

        var resultingArray = new Int64[hashArray.Length];

        var buffer = Array.Empty<Char>();

        var alphabet = _alphabet.CopyPooled();

        try
        {
            buffer = CreatePooledBuffer(_alphabet.Length, lottery);

            var startingIndex = 1 + _salt.Length;

            var length = _alphabet.Length - startingIndex;

            for (var i = 0; i < hashArray.Length; i++)
            {
                var subHashCharacter = hashArray[i];

                if (length > 0)
                {
                    Array.Copy(alphabet, 0, buffer, startingIndex, length);
                }

                ConsistentShuffle(alphabet, _alphabet.Length, buffer, _alphabet.Length);

                resultingArray[i] = Unhash(subHashCharacter, alphabet);
            }
        }
        finally
        {
            alphabet.ReturnArrayToPool();
            buffer.ReturnArrayToPool();
        }

        return EncodeLongs(resultingArray).Equals(hash)
            ? resultingArray : Array.Empty<Int64>();
    }

    private Char[] CreatePooledBuffer(Int32 alphabetLength, Char lottery)
    {
        var buffer = ArrayPool<Char>.Shared.Rent(alphabetLength);

        buffer[0] = lottery;

        Array.Copy(_salt, 0, buffer, 1, Math.Min(_salt.Length, alphabetLength - 1));

        return buffer;
    }

    private static void ConsistentShuffle(Char[] alphabet, Int32 alphabetLength, ReadOnlySpan<Char> salt,
        Int32 saltLength)
    {
        if (salt.Length == 0)
        {
            return;
        }

        var n = 0;

        for (int i = alphabetLength - 1, v = 0, p = 0; i > 0; i--, v++)
        {
            v %= saltLength;
            n = salt[v];
            p += n;

            var j = (n + v + p) % i;

            //Swaps characters
            (alphabet[j], alphabet[i]) = (alphabet[i], alphabet[j]);
        }
    }
    #endregion
    #region IUrlHasher Implementations
    public int DecodeSingleValue(string hash)
    {
        var numbers = GetNumbersFrom(hash);

        if (numbers.Length == 1)
        {
            return (int)numbers[0];
        }

        throw new InvalidOperationException("The hash value provided yielded more than one result.");
    }

    public long DecodeSingleLong(String hash)
    {
        var numbers = GetNumbersFrom(hash);

        if (numbers.Length == 1)
        {
            return numbers[0];
        }

        throw new InvalidOperationException("The hash provided yielded more than one result");
    }

    public (bool isSuccessful, int value) TryDecodeSingleValue(string hash)
    {
        var (isSuccessful, value) = (false, -1);

        var numbers = GetNumbersFrom(hash);

        if (numbers.Length == 1)
        {
            isSuccessful = true;
            value = (int)numbers[0];
        }

        return (isSuccessful, value);
    }


    public (bool isSuccessful, long value) TryDecodeSingleLong(string hash)
    {
        var (isSuccessful, value) = (false, -1L);

        var numbers = GetNumbersFrom(hash);

        if (numbers.Length == 1)
        {
            isSuccessful = true;
            value = (long)numbers[0];
        }

        return (isSuccessful, value);
    }


    public int[] DecodeValuesAsIntegers(string hash) => Array.ConvertAll(GetNumbersFrom(hash), n => (int)n);

    public long[] DecodeValuesAsLongs(string hash) => GetNumbersFrom(hash);

    public (bool isSuccessful, long value) TryDecodeValuesAsLongs(string hash)
    {
        throw new NotImplementedException();
    }

    public string DecodeHexValue(string hash)
    {
        var builder = StringBuilderPoolFactory<GameCodeGenerator>.Get(nameof(GameCodeGenerator));

        var numbers = DecodeValuesAsLongs(hash);

        foreach (var number in numbers)
        {
            var numberAsString = number.ToString("X");

            for (var i = 1; i < numberAsString.Length; i++)
            {
                builder.Append(numberAsString[i]);
            }
        }

        var resultingString = builder.ToString();

        return resultingString;
    }

    public string EncodeInteger(int number) => EncodeLongs(number);

    public string EncodeIntegers(IEnumerable<int> numbers) => EncodeIntegers(numbers.ToArray());

    public string EncodeIntegers(params int[] numbers) => GenerateHashFrom(Array.ConvertAll(numbers, n => (long)n));

    public string EncodeLong(long number)
    {
        ReadOnlySpan<long> span = stackalloc[] { number };

        return GenerateHashFrom(span);
    }

    public string EncodeLongs(IEnumerable<long> numbers) => EncodeLongs(numbers.ToArray());

    public string EncodeLongs(params long[] numbers) => GenerateHashFrom(numbers);

    public string EncodeHex(string hex)
    {
        if (String.IsNullOrWhiteSpace(hex) || !_hexValidator.Value.IsMatch(hex))
        {
            return String.Empty;
        }

        var matches = _hexSplitter.Value.Matches(hex);

        if (matches.Count == 0)
        {
            return String.Empty;
        }

        var numbers = new long[matches.Count];

        for (var i = 0; i < numbers.Length; i++)
        {
            var match = matches[i];

            var concat = String.Concat("1", match.Value);

            var number = Convert.ToInt64(concat, fromBase: 16);

            numbers[i] = number;
        }

        return EncodeLongs(numbers);
    }

    public ValueTask DisposeAsync()
    {
        StringBuilderPoolFactory<GameCodeGenerator>.Remove(nameof(GameCodeGenerator));

        return ValueTask.CompletedTask;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                StringBuilderPoolFactory<GameCodeGenerator>.Remove(nameof(GameCodeGenerator));
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}