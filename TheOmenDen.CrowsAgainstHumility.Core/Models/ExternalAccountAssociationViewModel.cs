namespace TheOmenDen.CrowsAgainstHumility.Core.Models;
public sealed record ExternalAccountAssociationViewModel
(String Username, String OriginalEmail , String AssociateEmail, Boolean AssociateExistingAccount, String LoginProvider, String ProviderDisplayName, String ProviderKey);