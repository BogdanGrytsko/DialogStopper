using HackatonEV2025;

var storage = new QuestionnaireGoogleSheetStorage();
var responses = await storage.GetAsync(1);

var prompt = "what can I get as a present for BDay for 10 y old boy?";

var caller = new OpenAIApiCaller();
var response = await caller.Execute(prompt);