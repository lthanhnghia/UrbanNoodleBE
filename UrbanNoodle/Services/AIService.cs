using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using UrbanNoodle.ApplicationContext;
using UrbanNoodle.Dto;
using UrbanNoodle.Repository.Interface;
using UrbanNoodle.Services.Interface;
namespace UrbanNoodle.Services
{
    public class AIService : IAlService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly IMemoryCache _cache;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingService;
        private readonly ILogger<AIService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IKnowledgeChunksRepository _knowledgeChunksRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ToolAlService _toolAlService;
        public AIService(Kernel kernel, IChatCompletionService chatCompletionService, IMemoryCache cache,
            IEmbeddingGenerator<string, Embedding<float>> embeddingService, ILogger<AIService> logger,
            ApplicationDbContext context, ToolAlService toolAlService,
            IKnowledgeChunksRepository knowledgeChunksRepository, IHttpContextAccessor httpContextAccessor)
        {
            _kernel = kernel;
            _chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();
            _cache = cache;
            _embeddingService = embeddingService;
            _logger = logger;
            _context = context;
            _knowledgeChunksRepository = knowledgeChunksRepository;
            _httpContextAccessor = httpContextAccessor;
            _toolAlService = toolAlService;
        }

        public async Task<ApiResponse> ChatAsync(string text, int? accountId)
        {
            GeminiPromptExecutionSettings geminiSettings = new()
            {
                FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),

            };

            var json = GetHistory();
            ChatHistory? chatHistory;
            if (!string.IsNullOrEmpty(json))
            {

                chatHistory = JsonSerializer.Deserialize<ChatHistory>(json);
                UpdateAuthState(chatHistory, accountId);
            }
            else
            {

                chatHistory = new ChatHistory();
                chatHistory.AddSystemMessage(BuildSystemInstruction(accountId));


            }

            int maxMessages = 12;
            chatHistory.AddUserMessage(text);

            while (chatHistory.Count > maxMessages)
            {
                // Luôn xóa ở index 1 để bảo vệ SystemInstruction ở index 0
                chatHistory.RemoveAt(1);
            }

            var vector = (await _embeddingService.GenerateAsync(new[] { text }))[0].Vector.ToArray();
            var contextChunks = await _knowledgeChunksRepository.SearchSimilarContextAsync(vector);



            string contextText = string.Join("\n", contextChunks);



            // 5. Tạo một bản copy tạm thời của history để gửi kèm ngữ cảnh (không lưu vào DB)
            var promptHistory = new ChatHistory();

            promptHistory.AddSystemMessage($"[NGỮ CẢNH TẠM THỜI]:\n{contextText}");

            foreach (var msg in chatHistory) { promptHistory.Add(msg); }
            _toolAlService.SetCurrentUser(accountId);
            _logger.LogInformation("AIService - ToolAlService HashCode: " + _toolAlService.GetHashCode());
            // 6. Gọi LLM (Giả sử bạn dùng _chatService)
            _logger.LogInformation("accountIds: " + accountId);
            var response = await _chatCompletionService.GetChatMessageContentAsync(
                     promptHistory,
                      executionSettings: geminiSettings,
                      kernel: _kernel
                 );


            // 7. Lưu kết quả thực sự vào lịch sử hội thoại (chỉ lưu đoạn đối thoại)
            chatHistory.AddAssistantMessage(response.Content);
            SaveHistory(JsonSerializer.Serialize(chatHistory));

            return new ApiResponse(200, response.Content);
        }




        private void SaveHistory(string json)
        {
            _httpContextAccessor.HttpContext?
                .Session
                .SetString("chat_history", json);
        }

        private string? GetHistory()
        {
            return _httpContextAccessor.HttpContext?
                .Session
                .GetString("chat_history");
        }
        private string BuildSystemInstruction(int? accountId)
        {
            string authLine = accountId.HasValue
                ? $"[AUTH]: ĐÃ ĐĂNG NHẬP | AccountId={accountId.Value}"
                : "[AUTH]: CHƯA ĐĂNG NHẬP";

            return
                $"{authLine}\n\n" +
                "Bạn là trợ lý ảo của quán ăn. Trả lời dựa trên [NGỮ CẢNH] và công cụ được cung cấp.\n\n" +
               "## NGUYÊN TẮC CỐT LÕI\n" +
"- Thông tin ngoài [NGỮ CẢNH]: Trả lời không biết, hướng dẫn gọi hotline.\n" +
"- Giọng điệu: Thân thiện, ngắn gọn, lễ phép.\n" +
"- TUYỆT ĐỐI không tự bịa/suy đoán thông tin khách hàng (tên, SĐT, địa chỉ).\n" +
"- Mọi thông tin khách hàng PHẢI lấy từ kết quả hàm get_customer_profile. không được tiết lộ ID của khách hàng\n\n" +
                "## PHÂN QUYỀN\n" +
                "- Xem menu / hỏi giá: KHÔNG cần đăng nhập.\n" +
                "- Đặt món / xem địa chỉ cũ: BẮT BUỘC đăng nhập.\n" +
                "- Nếu [AUTH] là CHƯA ĐĂNG NHẬP → yêu cầu đăng nhập trước, KHÔNG gọi bất kỳ hàm nào.\n" +
                "- Nếu [AUTH] là ĐÃ ĐĂNG NHẬP → tiến hành bình thường.\n\n" +
                "## GỌI HÀM\n" +
                "- Nếu lịch sử chat đã có dữ liệu menu → TUYỆT ĐỐI không gọi lại get_food.\n" +
                "- Gọi từng hàm tuần tự, KHÔNG gọi nhiều hàm cùng lúc.\n\n" +
               "## QUY TRÌNH ĐẶT MÓN (theo thứ tự)\n" +
"1. Khách muốn đặt món → GỌI NGAY get_customer_profile để lấy thông tin.\n" +
"2. TUYỆT ĐỐI không tự bịa họ tên, SĐT - chỉ dùng đúng data trả về từ get_customer_profile.\n" +
"3. Nếu khách có địa chỉ đã lưu → Hiển thị danh sách, hỏi chọn địa chỉ nào hoặc nhập mới.\n" +
"4. Nếu không có địa chỉ nào → Yêu cầu nhập địa chỉ mới.\n" +
"5. Thu thập đủ: món ăn, số lượng, địa chỉ giao hàng đã xác nhận.\n" +
"6. Nếu địa chỉ mơ hồ → hỏi lại, KHÔNG tự đoán.\n" +
"7. Tra ID món: BẮT BUỘC lấy đúng field 'id' từ dữ liệu menu. TUYỆT ĐỐI không tự gán ID theo thứ tự.\n" +
"8. Tóm tắt đơn hàng - CHỈ dùng thông tin thực tế từ get_customer_profile:\n" +
"   • Người nhận: [Fullname từ get_customer_profile] - [Phone từ get_customer_profile]\n" +
"   • Địa chỉ: [địa chỉ khách đã chọn hoặc nhập]\n" +
"   • [ID] - [Tên món] x [SL] = [Tiền] | Tổng tiền\n" +
"9. CHỈ gọi create_order khi khách nhắn: 'Đồng ý' / 'Xác nhận' / 'Approve'.\n\n" +
                "## TƯ VẤN COMBO\n" +
                "- Đọc kỹ mô tả từng món, chọn đúng theo tiêu chí khách (cay/hải sản/ngân sách...).\n" +
                "- Đề xuất DUY NHẤT 1 combo: Tên món chính xác + số lượng + tổng chi phí. Ngắn gọn, không rườm rà."


                ;
        }

        private void UpdateAuthState(ChatHistory chatHistory, int? accountId)
        {
            if (chatHistory == null || chatHistory.Count == 0) return;

            var systemMsg = chatHistory[0];
            if (systemMsg.Role != AuthorRole.System) return;

            var content = systemMsg.Content ?? "";

            string newAuthLine = accountId.HasValue
                ? $"[AUTH]: ĐÃ ĐĂNG NHẬP | AccountId={accountId.Value}"
                : "[AUTH]: CHƯA ĐĂNG NHẬP";

            var updated = System.Text.RegularExpressions.Regex.IsMatch(content, @"\[AUTH\]:")
                ? System.Text.RegularExpressions.Regex.Replace(content, @"\[AUTH\]:.*", newAuthLine)
                : newAuthLine + "\n\n" + content;

            chatHistory[0] = new ChatMessageContent(AuthorRole.System, updated);
        }
    }
}
