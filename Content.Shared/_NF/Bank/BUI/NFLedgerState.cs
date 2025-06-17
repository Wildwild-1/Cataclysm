using Content.Shared._NF.Bank.Components;
using Robust.Shared.Serialization;

namespace Content.Shared._NF.Bank.BUI;

[Serializable, NetSerializable]
public sealed class NFLedgerState : BoundUserInterfaceState
{
    public readonly NFLedgerEntry[] Entries;
    public NFLedgerState(NFLedgerEntry[] entries)
    {
        Entries = entries;
    }
}

[Serializable, NetSerializable]
public struct NFLedgerEntry
{
    public SectorBankAccount Account;
    public LedgerEntryType Type;
    public int Amount;
}

public enum LedgerEntryType : byte
{
    // Income entries
    TickingIncome,
    VendorTax,
    CargoTax,
    MailDelivered,
    AtmTax,
    ShipyardTax,
    // Mono begin
    BlackMarketSales,
    ColonialOutpostSales,
    TSFMCSales,
    MedicalSales,
    // Mono end
    BluespaceReward,
    AntiSmugglingBonus,
    MedicalBountyTax,
    StationDepositFines,
    StationDepositDonation,
    StationDepositAssetsSold,
    StationDepositOther,
    // Expense entries
    MailPenalty,
    // Mono Begin
    BlackMarketPenalties,
    ColonialOutpostPenalties,
    TSFMCPenalties,
    MedicalPenalties,
    // Mono End
    ShuttleRecordFees,
    StationWithdrawalPayroll,
    StationWithdrawalWorkOrder,
    StationWithdrawalSupplies,
    StationWithdrawalBounty,
    StationWithdrawalOther,
    // Utility values
    FirstExpense = MailPenalty,
}
