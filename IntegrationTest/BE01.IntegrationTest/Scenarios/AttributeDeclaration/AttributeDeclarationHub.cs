using BE01.IntegrationTest.Scenarios.AttributeDeclaration;
using Tapper;
using UiRtc.Public;
using UiRtc.Typing.PublicInterface;
using UiRtc.Typing.PublicInterface.Attributes;

namespace BE01.IntegrationTest.Scenarios.AttributeDeclaration
{
    [UiRtcHub("AttributeDeclaration")]
    public class AttributeDeclarationHub: IUiRtcHub
    {
    }

    public interface IAttributeDeclarationSender : IUiRtcSenderContract<AttributeDeclarationHub>
    {
        [UiRtcMethod("AttributeDeclarationAttributeAnswer")]
        Task AttributeDeclarationAnswer(AttributeDeclarationResponseMessage message);
    }

    [UiRtcMethod("AttributeDeclarationAttributeHandler")]
    public class AttributeDeclarationHandler(IUiRtcSenderService senderService) : IUiRtcHandler<AttributeDeclarationHub, AttributeDeclarationRequestMessage>
    {
        public async Task ConsumeAsync(AttributeDeclarationRequestMessage Model)
        {
            await senderService.Send<IAttributeDeclarationSender>().AttributeDeclarationAnswer(new AttributeDeclarationResponseMessage { CorrelationId = Model.CorrelationId });
        }
    }

    [TranspilationSource]
    public class AttributeDeclarationRequestMessage
    {
        public required string CorrelationId { get; set; }
    }

    [TranspilationSource]
    public class AttributeDeclarationResponseMessage
    {
        public required string CorrelationId { get; set; }
    }
}
