﻿using EventSourcing.Events;
using EventSourcing.Exceptions;
using EventSourcing.Models;

namespace EventSourcing;

public class AccountAggregate
{

  public string? AccountId { get; set; }
  public decimal Balance { get; set; }  
  public decimal MaxBalance { get; set; }
  public CurrencyType Currency { get; set; }
  public string? CustomerId { get; set; }
  public AccountStatus Status { get; set; }
  public List<LogMessage>? AccountLog { get; set; }

  private AccountAggregate(){}

  public static AccountAggregate? GenerateAggregate(Event[] events)
  {
    if (events.Length == 0)
    {
      return null;
    }
    
    var account = new AccountAggregate();
    foreach (var accountEvent in events)
    {
      account.Apply(accountEvent);
    }

    return account;
  }

  private void Apply(Event accountEvent)
  {
    switch (accountEvent)
    {
      case AccountCreatedEvent accountCreated:
        Apply(accountCreated);
        break;
      case DepositEvent deposit:
        Apply(deposit);
        break;
      case WithdrawalEvent wihdrawal:
        Apply(wihdrawal);
        break;
      case DeactivationEvent deactivation:
        Apply(deactivation);
        break;
      default:
        throw new EventTypeNotSupportedException("162 ERROR_EVENT_NOT_SUPPORTED");
    }
  }

  private void Apply(AccountCreatedEvent accountCreated)
  {
    AccountId = accountCreated.AccountId;
    Balance = accountCreated.InitialBalance;
    Currency = accountCreated.Currency;
    CustomerId = accountCreated.CustomerId;
    MaxBalance = accountCreated.MaxBalance;
  }



  private void Apply(DepositEvent deposit)
  {
    Balance += deposit.Amount;

    if (deposit.Amount > MaxBalance)
    {
     throw new Exception("281*");
    }     
  }

  private void Apply(WithdrawalEvent wihdrawal)
  {
    if(Balance == 0){
       throw new Exception("128*");
    }
    Balance -= wihdrawal.amount;

    if(Balance<0){
       throw new Exception("285*");  
    }
  }

  private void Apply(DeactivationEvent deactivation)
  {
    Status = AccountStatus.Disabled;

    if (deactivation.AccountId != null)
    {
      var log = new LogMessage("DEACTIVATE", deactivation.Reason.ToString(), deactivation.Timestamp);

      var logList = new List<LogMessage>();

      logList.Add(log);

      if(AccountLog != null){
AccountLog.AddRange(logList);
      }
      else{
        AccountLog = logList;
      }

      
    }
  }

  private void Apply(ActivationEvent activation)
  {
    throw new NotImplementedException();
  }

  private void Apply(CurrencyChangeEvent currencyChange)
  {
    throw new NotImplementedException();
  }

  private void Apply(ClosureEvent closure)
  {
    throw new NotImplementedException();
  }
}
