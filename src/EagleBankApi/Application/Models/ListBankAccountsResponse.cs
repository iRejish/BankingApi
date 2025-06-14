namespace EagleBankApi.Application.Models;

public class ListBankAccountsResponse
{
    public List<BankAccountResponse> Accounts { get; set; } = new List<BankAccountResponse>();
}
