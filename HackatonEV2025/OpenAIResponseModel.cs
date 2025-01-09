using System;
using System.Collections.Generic;

namespace HackatonEV2025;

public class OpenAIResponse
{
    public string Id { get; set; }
    public string Object { get; set; }
    public int Created { get; set; }
    public string Model { get; set; }
    public List<Choice> Choices { get; set; }
    public List<PromptFilterResult> PromptFilterResults { get; set; }
    public string SystemFingerprint { get; set; }
    public Usage Usage { get; set; }
}

public class Choice
{
    public ContentFilterResults ContentFilterResults { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
    public object Logprobs { get; set; }
    public Message Message { get; set; }
}

public class ContentFilterResults
{
    public Filter Hate { get; set; }
    public Filter ProtectedMaterialCode { get; set; }
    public Filter ProtectedMaterialText { get; set; }
    public Filter SelfHarm { get; set; }
    public Filter Sexual { get; set; }
    public Filter Violence { get; set; }
}

public class Filter
{
    public bool Filtered { get; set; }
    public string Severity { get; set; }
    public bool Detected { get; set; } // For fields that include detection
}

public class Message
{
    public string Content { get; set; }
    public object Refusal { get; set; }
    public string Role { get; set; }
}

public class PromptFilterResult
{
    public int PromptIndex { get; set; }
    public ContentFilterResults ContentFilterResults { get; set; }
}

public class Usage
{
    public int CompletionTokens { get; set; }
    public CompletionTokensDetails CompletionTokensDetails { get; set; }
    public int PromptTokens { get; set; }
    public PromptTokensDetails PromptTokensDetails { get; set; }
    public int TotalTokens { get; set; }
}

public class CompletionTokensDetails
{
    public int AcceptedPredictionTokens { get; set; }
    public int AudioTokens { get; set; }
    public int ReasoningTokens { get; set; }
    public int RejectedPredictionTokens { get; set; }
}

public class PromptTokensDetails
{
    public int AudioTokens { get; set; }
    public int CachedTokens { get; set; }
}
