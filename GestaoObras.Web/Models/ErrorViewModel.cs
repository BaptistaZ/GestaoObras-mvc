namespace GestaoObras.Web.Models
{
    /// Modelo padrão usado pela página Error.
    public sealed class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrWhiteSpace(RequestId);
    }
}