using System;
using System.Collections.Generic;
using System.Linq;
using Andrew_2_0_Libraries.FileHandling;

namespace Andrew_2_0_Libraries.Controllers
{
    public class FinanceController
    {
        List<FinanceTransaction> _transactions;
        List<MoneyDebt> _debts;

        FinanceFileHandler _financeFileHandler = new FinanceFileHandler();
        DebtFileHandler _debtFileHandler = new DebtFileHandler();

        /// <summary>
        /// Constructor
        /// </summary>
        public FinanceController()
        {
            // load transactions
            _transactions = _financeFileHandler.ReadFile<FinanceTransaction>();

            // load debts
            _debts = _debtFileHandler.ReadFile<MoneyDebt>();
        }

        #region Transactions
        /// <summary>
        /// Adds a new transaction
        /// </summary>
        /// <param name="value">The value of the transaction</param>
        /// <param name="title">The title/summary of the transaction</param>
        /// <param name="isIncome">Whether this transaction is an income (false if outgoing)</param>
        /// <param name="frequency">How often this transaction takes place</param>
        /// <param id="description">Description of the debt</param>
        public void AddTransaction(float value, string title, bool isIncome, TransactionFrequency frequency, string description)
        {
            var transaction = new FinanceTransaction(value, title, isIncome, frequency, description);
            _transactions.Add(transaction);
            Save_();
        }

        /// <summary>
        /// Adds a new transaction
        /// </summary>
        /// <param name="data">The data of the transaction</param>
        public void AddTransaction(FinanceTransaction ft)
        {
            _transactions.Add(ft);
            Save_();
        }

        /// <summary>
        /// Updates an existing transaction
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to update</param>
        /// <param name="value">The new value of the transaction</param>
        /// <param name="title">The new title/summary of the transaction</param>
        /// <param name="isIncome">Whether this transaction is an income (false if outgoing)</param>
        /// <param name="frequency">How often this transaction takes place</param>
        /// <param id="description">Description of the debt</param>
        public void UpdateTransaction(Guid transactionId, float value, string title, bool isIncome, TransactionFrequency frequency, string description)
        {
            // find transaction with correct ID
            var matching = _transactions.Where(r => r.GetTransactionId() == transactionId).FirstOrDefault();
            if (matching != null)
            {
                matching.UpdateTransaction(value, title, isIncome, frequency, description);
            }
            else
            {
                // if unable to find the matching transaction, add a new one
                AddTransaction(value, title, isIncome, frequency, description);
            }
            Save_();
        }

        /// <summary>
        /// Deletes the specified transaction
        /// </summary>
        /// <param name="transactionId">The ID of the transaction to delete</param>
        public void DeleteTransaction(Guid transactionId)
        {
            for (int i = 0; i < _transactions.Count; i++)
            {
                // find and remove the specified transaction
                if (_transactions[i].GetTransactionId() == transactionId)
                {
                    _transactions.RemoveAt(i);
                    break;
                }
            }
            Save_();
        }

        /// <summary>
        /// Gets all incoming transactions
        /// </summary>
        public List<FinanceTransaction> GetIncomeTransactions()
        {
            var incomes = _transactions.Where(t => t.IsIncome() == true).ToList();
            return incomes;
        }

        /// <summary>
        /// Gets all outgoing transactions
        /// </summary>
        public List<FinanceTransaction> GetOutgoingTransactions()
        {
            var outgoings = _transactions.Where(t => t.IsIncome() == false).ToList();
            return outgoings;
        }
        #endregion

        #region Debts
        /// <summary>
        /// Adds a new transaction
        /// </summary>
        /// <param id="value">The monetary value of the transaction</param>
        /// <param id="otherParty">Who owes/is owed money</param>
        /// <param id="isIncome">Whether the transaction is an income (false if outgoing)</param>
        /// <param id="description">Description of the debt</param>
        public void AddDebt(float value, string otherParty, bool isIncome, string description)
        {
            var debt = new MoneyDebt(value, otherParty, isIncome, description);
            _debts.Add(debt);
            Save_();
        }
        /// <summary>
        /// Adds a new transaction
        /// </summary>
        /// <param id="data">The data of the transaction</param>
        public void AddDebt(MoneyDebt md)
        {
            _debts.Add(md);
            Save_();
        }

        /// <summary>
        /// Updates an existing transaction
        /// </summary>
        /// <param id="debtid">The ID of the debt to be updated</param>
        /// <param id="value">The monetary value of the transaction</param>
        /// <param id="otherParty">Who owes/is owed money</param>
        /// <param id="isIncome">Whether the transaction is an income (false if outgoing)</param>
        /// <param id="description">Description of the debt</param>
        public void UpdateDebt(Guid debtId, float value, string otherParty, bool isIncome, string description)
        {
            // find transaction with correct ID
            var matching = _debts.Where(r => r.GetTransactionId() == debtId).FirstOrDefault();
            if (matching != null)
            {
                matching.UpdateTransaction(value, otherParty, isIncome, description);
            }
            else
            {
                // if unable to find the matching transaction, add a new one
                AddDebt(value, otherParty, isIncome, description);
            }
            Save_();
        }

        /// <summary>
        /// Resolves the specified debt
        /// </summary>
        /// <param id="debtId">The ID of the debt to resolve</param>
        /// <param id="date">The date that the debt was resolved on</param>
        public void ResolveDebt(Guid debtId, DateTime date)
        {
            // find transaction with correct ID
            var matching = _debts.Where(r => r.GetTransactionId() == debtId).FirstOrDefault();
            if (matching != null)
            {
                matching.Resolved(date);
            }
            Save_();
        }

        /// <summary>
        /// Gets a list of debts
        /// </summary>
        /// <param id="resolved">The state to get matching debts for</param>
        /// <returns>A list of debts that are resolved/unresolved, depending on input param</returns>
        public List<MoneyDebt> GetDebts(bool resolved)
        {
            var matching = _debts.Where(r => r.IsResolved() == resolved).ToList();
            return matching;
        }
        #endregion

        /// <summary>
        /// Saves all jokes and comments to file
        /// </summary>
        void Save_()
        {
            _financeFileHandler.WriteFile(_transactions);
            _debtFileHandler.WriteFile(_debts);
        }
    }
}
