using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Azure_Dz_4.Models;
using Azure_Dz_4.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Azure_Dz_4.Controllers
{
    public class MessageController : Controller
    {
        private readonly string queueName = "currency";
        private readonly IConfiguration configuration;
        private readonly string connStr;

        public MessageController(IConfiguration configuration)
        {
            this.configuration = configuration;
            connStr = configuration.GetSection("AZURE_STORAGE_CONNECTION_STRING").Value;
        }
        public async Task<IActionResult> Index(MessageIndexViewModel messageIndexViewModel)
        {
            QueueClient queueClient = await CreateQueueClient(connStr, queueName);
            var azureResponse = (await queueClient.PeekMessagesAsync(maxMessages: 10));
            PeekedMessage[] peekedMessages = azureResponse.Value;
            List<PurchaseLotDTO> purchaseLot = new List<PurchaseLotDTO>();
            foreach (var message in peekedMessages)
            {
                purchaseLot.Add(JsonSerializer.Deserialize<PurchaseLotDTO>(message.Body));
            }
            if (messageIndexViewModel.CurrencyName == null)
            {
                messageIndexViewModel.PurchaseLotDTOs = purchaseLot;
                return View(messageIndexViewModel);           
            }

            IEnumerable<PurchaseLotDTO> selected = purchaseLot.Where(p => p.CurrencyName == messageIndexViewModel.CurrencyName);
            messageIndexViewModel.PurchaseLotDTOs = selected;
            return View(messageIndexViewModel);
        }



        public IActionResult Create()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseLotDTO message)
        {
            if (!ModelState.IsValid)
                return View();
            QueueClient queue = await CreateQueueClient(connStr, queueName);
            var json = JsonSerializer.Serialize(message);
            SendReceipt receipt = await queue.SendMessageAsync(json,
                visibilityTimeout: TimeSpan.FromSeconds(1),
                timeToLive: TimeSpan.FromDays(1));
            return RedirectToAction("Index", "Home");
        }

        public async Task<QueueClient> CreateQueueClient(string connStr, string queueName)
        {
            QueueServiceClient queueService = new QueueServiceClient(connStr);
            QueueClient queue = queueService.GetQueueClient(queueName);
            await queue.CreateIfNotExistsAsync();
            return queue;
        }
    }
}
