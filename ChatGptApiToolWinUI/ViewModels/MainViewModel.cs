using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OpenAI.Interfaces;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;

namespace ChatGptApiToolWinUI.ViewModels;

public partial class MainViewModel : ObservableRecipient
{
    private readonly IOpenAIService _openAiService;


    [ObservableProperty] private string? _inputText;
    private readonly ILogger _logger;

    public MainViewModel(ILogger<MainViewModel> logger, IOpenAIService openAiService)
    {
        _logger = logger;
        _openAiService = openAiService;

    }

    [RelayCommand]
    private async Task CallApi()
    {
        if (string.IsNullOrEmpty(InputText)) return;

        _logger.LogInformation($"{InputText}");

        var result = await _openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest()
        {
            Messages = new List<ChatMessage>()
            {
                ChatMessage.FromUser(InputText),
            },
            Model = Models.Gpt_3_5_Turbo_16k,
        });

        if (!result.Successful)
        {
            var errorMessage = $"{string.Join(",", result.Error?.Message, result.Error?.Code, result.Error?.Type, result.Error?.Param)}";
            _logger.LogWarning(errorMessage);
            InputText += $"{Environment.NewLine}{errorMessage}";
            return;
        }

        foreach (var choice in result.Choices)
        {
            _logger.LogInformation($"{choice.Message.Content}");
            InputText += $"{Environment.NewLine}{choice.Message.Content}";
        }
        
    }
}
