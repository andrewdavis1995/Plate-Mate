using Andrew_2_0_Libraries.Models;
using System;
using System.Globalization;

namespace Andrew_2_0_Libraries.Controllers
{
    public class MoneyDebt : BaseSaveable
    {
        float _value;
        bool _income;   // if false, this is a payment/outgoing
        string _otherParty;
        string _description;
        Guid _transactionId;
        bool _resolved;
        DateTime _dateResolved;

        #region Accessor functions
        public float GetValue() { return _value; }
        public string GetOtherParty() { return _otherParty; }
        public string GetDescription() { return _description; }
        public Guid GetTransactionId() { return _transactionId; }
        public bool IsIncome() { return _income; }
        public bool IsResolved() { return _resolved; }
        public DateTime GetDateResolved() { return _dateResolved; }
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MoneyDebt() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param id="value">The monetary value of the transaction</param>
        /// <param id="otherParty">Who owes/is owed money</param>
        /// <param id="isIncome">Whether the transaction is an income (false if outgoing)</param>
        /// <param id="description">Description of the debt</param>
        public MoneyDebt(float value, string otherParty, bool isIncome, string description)
        {
            UpdateTransaction(value, otherParty, isIncome, description);
            _transactionId = Guid.NewGuid();
        }

        /// <summary>
        /// Updates the values stored in this existing transaction
        /// </summary>
        /// <param id="value">The monetary value of the transaction</param>
        /// <param id="otherParty">Who owes/is owed money</param>
        /// <param id="isIncome">Whether the transaction is an income (false if outgoing)</param>
        /// <param id="description">Description of the debt</param>
        internal void UpdateTransaction(float value, string otherParty, bool isIncome, string description)
        {
            _value = value;
            _otherParty = otherParty;
            _income = isIncome;
            _description = description;
        }

        /// <summary>
        /// Marks the payment as resolved
        /// </summary>
        /// <param id="dateResolved">When the debt was resolved</param>
        public void Resolved(DateTime dateResolved)
        {
            _resolved = true;
            _dateResolved = dateResolved;
        }

        /// <summary>
        /// Marks the payment as resolved
        /// </summary>
        /// <param id="dateResolved">[Output]When the debt was resolved</param>
        /// <returns>Whether the payment is resolved</returns>
        internal bool IsResolved(ref DateTime dateResolved)
        {
            // update reference
            if (_resolved)
                dateResolved = _dateResolved;

            return _resolved;
        }

        /// <summary>
        /// Gets the output to be written to the save file
        /// </summary>
        /// <returns>File output</returns>
        public override string GetTextOutput()
        {
            return _value.ToString() + "@" + _income + "@" + _otherParty + "@" + _description + "@" + _transactionId.ToString() + "@" + _resolved + "@" + _dateResolved.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// Parses the data into a Target object
        /// </summary>
        /// <param name="data">The data to parse</param>
        public override bool ParseData(string data)
        {
            bool success = false;
            var split = data?.Split('@');

            if (split.Length > 6)
            {
                try
                {
                    _value = float.Parse(split[0]);
                    _income = bool.Parse(split[1]);
                    _otherParty = split[2];
                    _description = split[3];
                    _transactionId = Guid.Parse(split[4]);
                    _resolved = bool.Parse(split[5]);
                    _dateResolved = DateTime.ParseExact(split[6], "dd/MM/yyyy", CultureInfo.InvariantCulture);

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
