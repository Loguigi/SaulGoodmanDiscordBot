using System.ComponentModel.DataAnnotations.Schema;

namespace GarryLibrary.Models;

public interface IPageable
{
    string GetPageItemDisplay(string context = "");
    
}