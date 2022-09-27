using Andrew_2_0_Libraries.Models;
using System;

namespace Andrew_2_0_Libraries.Controllers
{
    public enum TransactionFrequency { Daily, Weekly, Monthly, Quarterly, Yearly };

    public class FinanceTransaction : BaseSaveable
    {
        float _value;
        bool _income;   // if false, this is a payment/outgoing
        string _name;
        Guid _transactionId;
        TransactionFrequency _frequency;
        string _description;
        
        #region Accessor functions
        public float GetValue() { return _value; }
        public string GetTransactionName() { return _name; }
        public string GetDescription() { return _description; }
        public Guid GetTransactionId() { return _transactionId; }
        public bool IsIncome() { return _income; }
        public TransactionFrequency GetFrequency() { return _frequency; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public FinanceTransaction() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param id="value">The monetary value of the transaction</param>
        /// <param id="title">The title/description of the transaction</param>
        /// <param id="isIncome">Whether the transaction is an income (false if outgoing)</param>
        /// <param id="frequency">How often this payment/expenditure occurs</param>
        /// <param id="description">Description of the debt</param>
        public FinanceTransaction(float value, string title, bool isIncome, TransactionFrequency frequency, string description)
        {
            UpdateTransaction(value, title, isIncome, frequency, description);
            _transactionId = Guid.NewGuid();
        }

        /// <summary>
        /// Updates the values stored in this existing transaction
        /// </summary>
        /// <param id="value">The monetary value of the transaction</param>
        /// <param id="title">The title/description of the transaction</param>
        /// <param id="isIncome">Whether the transaction is an income (false if outgoing)</param>
        /// <param id="frequency">How often this payment/expenditure occurs</param>
        /// <param id="description">Description of the debt</param>
        internal void UpdateTransaction(float value, string title, bool isIncome, TransactionFrequency frequency, string description)
        {
            _value = value;
            _name = title;
            _income = isIncome;
            _frequency = frequency;
            _description = description;
        }

        /// <summary>
        /// Gets the output to be written to the save file
        /// </summary>
        /// <returns>File output</returns>
        public override string GetTextOutput()
        {
            return _value.ToString() + "@" + _income + "@" + _name + "@" + _transactionId.ToString() + "@" + _frequency + "@" + _description;
        }

        /// <summary>
        /// Parses the data into a Target object
        /// </summary>
        /// <param name="data">The data to parse</param>
        public override bool ParseData(string data)
        {
            bool success = false;
            var split = data?.Split('@');

            if (split.Length > 5)
            {
                try
                {
                    _value = float.Parse(split[0]);
                    _income = bool.Parse(split[1]);
                    _name = split[2];
                    _transactionId = Guid.Parse(split[3]);
                    _frequency = (TransactionFrequency)Enum.Parse(typeof(TransactionFrequency), split[4]);
                    _description = split[5];
                    success = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    success = false;
                }
            }

            return success;
        }
    }
}
