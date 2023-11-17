using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using WukkamanCleaningAgencyFrontend.Models;
using System.Text;
using System.Net.Http.Headers;

namespace WukkamanCleaningAgencyFrontend.Controllers
{
    public class ShiftController : Controller
    {
        private readonly IHttpClientFactory _clientHandler;
        public ShiftController(IHttpClientFactory clientHandler)
        {
            this._clientHandler = clientHandler;
        }

        public async Task<IActionResult> Index()
        {
            HttpClient httpClient = _clientHandler.CreateClient("ShiftAPI");

            HttpResponseMessage response = await httpClient.GetAsync("");

            List<Shift> productList = new();

            if(response != null){

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();

                productList = JsonConvert.DeserializeObject<List<Shift>>(content)!;
            }
            else
                return Problem("Error in Api response");

            return View(productList);
            }
            else 
                return View(productList);
        }


        public IActionResult Upsert(int id = 0)
        {

            if(id == 0)
                return View(new Shift());
            else
            {
                HttpResponseMessage ShiftResponse = _clientHandler.CreateClient("ShiftAPI").GetAsync($"{id}").Result;
                string Shift = ShiftResponse.Content.ReadAsStringAsync().Result;

                Shift view = JsonConvert.DeserializeObject<Shift>(Shift)!;
                
                return View(view);
            }
        }


        [HttpPost]

        public IActionResult Upsert(Shift Shift)
        {
           if (!ModelState.IsValid) return View(Shift);

           string json = JsonConvert.SerializeObject(Shift);

           StringContent data = new(json, Encoding.UTF8, "application/json");

           if (Shift.Id == 0)
           {
               HttpResponseMessage response = _clientHandler.CreateClient("ShiftAPI").PostAsync("", data).Result;

               if (response.IsSuccessStatusCode)
               {
                   return RedirectToAction("Index");
               }
               else
               {
                   ModelState.AddModelError(string.Empty, "Product creation failed");
                   return View(Shift);
               }
           }
           else
           {
               HttpResponseMessage response = _clientHandler.CreateClient("ShiftAPI").PutAsync($"{Shift.Id}", data).Result;

               if (response.IsSuccessStatusCode)
               {
                   return RedirectToAction("Index");
               }
               else
               {
                   ModelState.AddModelError(string.Empty, "Product creation failed");
                   return View(Shift);
               }
           }
        }

       

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            HttpClient httpClient = _clientHandler.CreateClient("ShiftAPI");
            HttpResponseMessage response = await httpClient.DeleteAsync($"{id}");
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Student Delete failed");
                return RedirectToAction("Index");
            }
        }


    }
}