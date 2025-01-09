using DialogStopper.Storage;

namespace HackatonEV2025;

public class QuestionnaireGoogleSheetStorage : GoogleSheetStorage<QuestionnaireResponse>
{
    private const string sheetId = "1o1Yi_xWMEcN0ki5zy6Yk_NsYOCBC52ix9t57T_ulMn8";

    public QuestionnaireGoogleSheetStorage() : base(sheetId)
    {
        SheetName = "EV Hackaton 2025";
    }
}
