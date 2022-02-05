using Google.Apis.Sheets.v4;
using System;

namespace DialogStopper
{
    public class GoogleSheetStorage
    {
        public static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };

        public void SyncAllHistory()
        {
        }
    }
}
