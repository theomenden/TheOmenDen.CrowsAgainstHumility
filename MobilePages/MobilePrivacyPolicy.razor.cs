
using Blazorise.LoadingIndicator;
using Microsoft.JSInterop;

namespace TheOmenDen.CrowsAgainstHumility.MobilePages;

public partial class MobilePrivacyPolicy: ComponentBase
{
    private LoadingIndicator _loadingIndicator;
    private const string PrivacyScriptUrl = @"https://stores.enzuzo.com/__enzuzo-privacy-app.js?mode=privacy&apiHost=https://stores.enzuzo.com&buttonStyle=%0A%7B%0A%20%20%22buttonWidget%22%3A%20%7B%0A%20%20%20%20%22backgroundColor%22%3A%20%22%23ffffff%22%2C%0A%20%20%20%20%22color%22%3A%20%22%23000000%22%2C%0A%20%20%20%20%22%26%3Ahover%22%3A%20%7B%0A%20%20%20%20%20%20%22backgroundColor%22%3A%20%22%23a4a4a4%22%2C%0A%20%20%20%20%20%20%22color%22%3A%20%22%23000000%22%0A%20%20%20%20%7D%0A%20%20%7D%0A%7D%0A&qt=1663256104966&referral=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJDdXN0b21lcklEIjo2NjY3LCJDdXN0b21lck5hbWUiOiJzaG9waWZ5LXRoZS1vbWVuLWRlbi5teXNob3BpZnkuY29tIiwiQ3VzdG9tZXJMb2dvVVJMIjoiIiwiUm9sZXMiOlsicmVmZXJyYWwiXSwiUHJvZHVjdCI6InNob3BpZnkiLCJpc3MiOiJFbnp1em8gSW5jLiIsIm5iZiI6MTY2MzI1NTg2M30.5ltsDun8hHbVHtim61An_rpsigNtFtMFHaTirBHpnzo";
    private const string PrivacyScriptId = @"__enzuzo-root-script";
    private const string ImagePath = @"https://www.positivessl.com/images/seals/positivessl_trust_seal_lg_222x54.png";
    private const string CertLink = @"javascript:if(window.open('https://secure.trust-provider.com/ttb_searcher/trustlogo?v_querytype=W&v_shortname=POSEV&v_search=https://crowsagainsthumility.app/&x=6&y=5','tl_wnd_credentials'+(new Date()).getTime(),'toolbar=0,scrollbars=1,location=1,status=1,menubar=1,resizable=1,width=374,height=660,left=60,top=120')){};tLlB(tLTB);";

    [Inject] private IJSRuntime JsRuntime { get; init; }
     
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await _loadingIndicator.Show();
        await JsRuntime.InvokeVoidAsync("loadJsById", PrivacyScriptId, PrivacyScriptUrl);
        await _loadingIndicator.Hide();
    }
}
