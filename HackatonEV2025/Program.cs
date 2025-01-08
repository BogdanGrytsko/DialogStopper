using HackatonEV2025;
using System.Text;

var storage = new QuestionnaireGoogleSheetStorage();
var responses = await storage.GetAsync();

var caller = new OpenAIApiCaller();
var prompt = "Hey, I have a questionnaire which is filled by users." +
    $"Here are the questions: {Questions.Q1} {Questions.Q2} {Questions.Q3} {Questions.Q4} {Questions.Q5} {Questions.Q6} {Questions.Q7} {Questions.Q8} {Questions.Q9} {Questions.Q10} {Questions.Q11}" + Environment.NewLine +
    "For questions 1-10 user gives an answer with a number, ranging from 1 to 10. In 11th question user shares a free form text." + Environment.NewLine +
    "Could you analyze the text and based on it suggest in which questions(1-10) numeric answer isn't actually reflecting what is in free text, and what an approprioate number would be?" + Environment.NewLine +
    "I'll give an example: Question is '9. How would you rate the accountability of the company's leadership in upholding ESG principles?', and user has answered 10. However in free text Q11 he writes down: 'A well-rounded approach to ESG practices, but leadership accountability needs strengthening.'" + Environment.NewLine +
    "In such case there is a discrepancy, and suggested score(by you) could be 4." + Environment.NewLine +
    "Below I provide rows with user answers. Please provide you comments and suggested score also in rows, keeping the rowId." + Environment.NewLine +
    "Please respond in the following format (1st row as an example) : \r\n" +
    "| rowId | Comments | Suggested Score |\r\n|-------|----------|------------------|\r\n| 1     | your explanation | Q3: 5 (instead of 3), Q10: 5 (instead of 2) |";

int rowId = 1;
foreach (var responseBatch in responses.Chunk(10))
{
    var sb = new StringBuilder(prompt);
    foreach(var response in responseBatch)
    {
        sb.AppendLine(response.GetString(rowId++));
    }
    var finalPrompt = sb.ToString();
    var openAiResponse = await caller.Execute(finalPrompt);
    var message = openAiResponse.Choices.First().Message.Content;
    var parsedResponse = RowAnalysisParser.ParseResponse(message);
}