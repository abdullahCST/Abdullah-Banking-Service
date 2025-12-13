using System;
using System.Collections.Generic;
using System.IO;

namespace AbdullahBankingService
{
    public class CustomerAccount
    {
        public string AccountNumber { get; }
        public string FullName { get; set; }
        private string AccountPin { get; set; }
        public decimal Balance { get; private set; }
        public DateTime LastLogin { get; set; }
        public DateTime AccountCreated { get; }
        private List<string> TransactionHistory { get; }
        public string StudentID { get; set; }
        public string University { get; set; }
        public string SecurityQuestion { get; private set; }
        public string SecurityAnswer { get; private set; }
        public bool CardLocked { get; private set; }
        public bool TransactionAlerts { get; private set; }

        public CustomerAccount(string accNum, string name, string pin, decimal initialDeposit)
        {
            AccountNumber = accNum;
            FullName = name;
            AccountPin = pin;
            Balance = initialDeposit;
            AccountCreated = DateTime.Now;
            LastLogin = DateTime.Now;
            StudentID = "2442130258";
            University = "Nantong University";
            SecurityQuestion = "";
            SecurityAnswer = "";
            CardLocked = false;
            TransactionAlerts = true;
            TransactionHistory = new List<string>
            {
                $"[{DateTime.Now:yyyy-MM-dd HH:mm}] ACCOUNT CREATED. Initial Deposit: {initialDeposit}"
            };
        }

        public bool CheckPin(string inputPin)
        {
            return inputPin == AccountPin;
        }

        public void ChangePin(string oldPin, string newPin)
        {
            if (oldPin == AccountPin)
            {
                AccountPin = newPin;
                TransactionHistory.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] PIN CHANGED");
            }
        }

        public void SetSecurityQuestion(string question, string answer)
        {
            SecurityQuestion = question;
            SecurityAnswer = answer;
        }

        public bool VerifySecurityAnswer(string answer)
        {
            return answer == SecurityAnswer;
        }

        public void ToggleCardLock(bool lockCard)
        {
            CardLocked = lockCard;
            string status = lockCard ? "LOCKED" : "UNLOCKED";
            TransactionHistory.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] CARD {status}");
        }

        public void ToggleTransactionAlerts(bool enable)
        {
            TransactionAlerts = enable;
        }

        public bool MakeDeposit(decimal amount)
        {
            if (amount <= 0 || amount > 50000)
            {
                Console.WriteLine("   Invalid deposit amount.");
                return false;
            }

            Balance += amount;
            TransactionHistory.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] DEPOSIT: +{amount}. New Balance: {Balance}");

            if (TransactionAlerts)
            {
                Console.WriteLine($"   [ALERT]: Deposit of {amount} CNY received at {DateTime.Now:HH:mm}");
            }

            Console.WriteLine($"   Deposited {amount}");
            return true;
        }

        public bool MakeWithdrawal(decimal amount, string pin)
        {
            if (!CheckPin(pin))
            {
                Console.WriteLine("   Invalid PIN.");
                return false;
            }

            if (CardLocked)
            {
                Console.WriteLine("   Card is locked. Please visit branch to unlock.");
                return false;
            }

            if (amount <= 0 || amount > 20000)
            {
                Console.WriteLine("   Invalid withdrawal amount.");
                return false;
            }

            if (amount > Balance)
            {
                Console.WriteLine("   Insufficient funds.");
                return false;
            }

            if (amount > Balance * 0.7m)
            {
                Console.Write($"   Large withdrawal warning. Confirm? (Y/N): ");
                string confirm = Console.ReadLine();
                if (confirm.ToUpper() != "Y")
                {
                    Console.WriteLine("   Cancelled.");
                    return false;
                }
            }

            Balance -= amount;
            TransactionHistory.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] WITHDRAWAL: -{amount}. New Balance: {Balance}");

            if (TransactionAlerts)
            {
                Console.WriteLine($"   [ALERT]: Withdrawal of {amount} CNY made at {DateTime.Now:HH:mm}");
            }

            Console.WriteLine($"   Withdrew {amount}");
            return true;
        }

        public void ShowAccountInfo()
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("              ACCOUNT DETAILS");
            Console.WriteLine("==================================================");
            Console.WriteLine($"   Account: {AccountNumber}");
            Console.WriteLine($"   Holder: {FullName}");
            Console.WriteLine($"   Balance: {Balance}");
            Console.WriteLine($"   Student ID: {StudentID}");
            Console.WriteLine($"   University: {University}");
            Console.WriteLine($"   Card Status: {(CardLocked ? "LOCKED" : "Active")}");
            Console.WriteLine($"   Created: {AccountCreated:yyyy-MM-dd}");
            Console.WriteLine($"   Last Login: {LastLogin:yyyy-MM-dd HH:mm}");
            Console.WriteLine("==================================================\n");
        }

        public void GenerateStatement()
        {
            string fileName = $"Statement_{AccountNumber}_{DateTime.Now:yyyyMMdd}.txt";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(desktopPath, fileName);

            try
            {
                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    writer.WriteLine("==================================================");
                    writer.WriteLine("            ABDULLAH BANKING SERVICE");
                    writer.WriteLine("               ACCOUNT STATEMENT");
                    writer.WriteLine("==================================================");
                    writer.WriteLine($"Account: {FullName}");
                    writer.WriteLine($"Number: {AccountNumber}");
                    writer.WriteLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm}");
                    writer.WriteLine($"Balance: {Balance}");
                    writer.WriteLine("--------------------------------------------------");
                    writer.WriteLine("TRANSACTIONS:");

                    foreach (string transaction in TransactionHistory)
                    {
                        writer.WriteLine(transaction);
                    }

                    writer.WriteLine("==================================================");
                }
                Console.WriteLine($"   Statement saved to Desktop: {fileName}");
            }
            catch
            {
                Console.WriteLine("   Could not save statement.");
            }
        }

        public List<string> GetTransactionHistory()
        {
            return TransactionHistory;
        }

        public void ReceiveTransfer(decimal amount, string senderName)
        {
            Balance += amount;
            TransactionHistory.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] TRANSFER RECEIVED: +{amount} from {senderName}. New Balance: {Balance}");

            if (TransactionAlerts)
            {
                Console.WriteLine($"   [ALERT]: Transfer of {amount} CNY received from {senderName} at {DateTime.Now:HH:mm}");
            }
        }

        public bool MakePayment(decimal amount, string serviceType, string reference)
        {
            if (!MakeWithdrawal(amount, "0000"))
            {
                return false;
            }

            TransactionHistory.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] PAYMENT: -{amount} for {serviceType} (Ref: {reference})");
            return true;
        }
    }

    public class HelpDesk
    {
        private static List<string> complaints = new List<string>();

        public static void ReportLostCard(CustomerAccount account)
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("           REPORT LOST/STOLEN CARD");
            Console.WriteLine("==================================================");
            Console.WriteLine($"   Account: {account.AccountNumber}");
            Console.WriteLine($"   Holder: {account.FullName}");
            Console.Write("   When did you lose it? ");
            string when = Console.ReadLine();
            Console.Write("   Last location used? ");
            string location = Console.ReadLine();

            account.ToggleCardLock(true);

            string ticket = $"LOST-CARD-{DateTime.Now:yyyyMMddHHmmss}";
            string complaint = $"[{DateTime.Now}] Ticket: {ticket} | Account: {account.AccountNumber} | Type: Lost Card | When: {when} | Location: {location}";
            complaints.Add(complaint);

            SaveComplaintToFile(complaint);

            Console.WriteLine("\n   ✓ Card locked for security");
            Console.WriteLine($"   ✓ Ticket created: {ticket}");
            Console.WriteLine("   ✓ Visit any branch with your passport to get replacement");
            Console.WriteLine("   ✓ Contact: +86 1772 164 6613 (24/7 Lost Card Hotline)");
        }

        public static void ReportTransactionIssue(CustomerAccount account)
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("           REPORT TRANSACTION ISSUE");
            Console.WriteLine("==================================================");
            Console.Write("   Transaction Date (YYYY-MM-DD): ");
            string date = Console.ReadLine();
            Console.Write("   Transaction Amount: ");
            string amount = Console.ReadLine();
            Console.Write("   Issue Description: ");
            string description = Console.ReadLine();

            string ticket = $"TRX-{DateTime.Now:yyyyMMddHHmmss}";
            string complaint = $"[{DateTime.Now}] Ticket: {ticket} | Account: {account.AccountNumber} | Type: Transaction Issue | Date: {date} | Amount: {amount} | Issue: {description}";
            complaints.Add(complaint);

            SaveComplaintToFile(complaint);

            Console.WriteLine($"\n   ✓ Ticket created: {ticket}");
            Console.WriteLine("   ✓ Our team will investigate within 2 business days");
            Console.WriteLine("   ✓ You will receive SMS update");
        }

        public static void SubmitGeneralInquiry(CustomerAccount account)
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("              GENERAL INQUIRY");
            Console.WriteLine("==================================================");
            Console.WriteLine("   1. Account Opening Requirements");
            Console.WriteLine("   2. International Transfer Help");
            Console.WriteLine("   3. PSB Registration Support");
            Console.WriteLine("   4. Student Loan Information");
            Console.WriteLine("   5. Other");
            Console.Write("\n   Select inquiry type: ");
            string type = Console.ReadLine();

            Console.Write("   Your message: ");
            string message = Console.ReadLine();

            string ticket = $"INQ-{DateTime.Now:yyyyMMddHHmmss}";
            string complaint = $"[{DateTime.Now}] Ticket: {ticket} | Account: {account.AccountNumber} | Type: Inquiry-{type} | Message: {message}";
            complaints.Add(complaint);

            SaveComplaintToFile(complaint);

            Console.WriteLine($"\n   ✓ Ticket created: {ticket}");
            Console.WriteLine("   ✓ We'll respond within 24 hours via email");
            Console.WriteLine("   ✓ Check your registered email for updates");
        }

        public static void ViewComplaintStatus()
        {
            Console.WriteLine("\n==================================================");
            Console.WriteLine("           COMPLAINT STATUS");
            Console.WriteLine("==================================================");

            if (complaints.Count == 0)
            {
                Console.WriteLine("   No complaints submitted.");
                return;
            }

            foreach (var complaint in complaints)
            {
                Console.WriteLine($"   • {complaint}");
            }
        }

        private static void SaveComplaintToFile(string complaint)
        {
            string fileName = "complaints_log.txt";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(desktopPath, fileName);

            try
            {
                using (StreamWriter writer = File.AppendText(fullPath))
                {
                    writer.WriteLine(complaint);
                }
            }
            catch
            {
            }
        }
    }

    class Program
    {
        private static List<CustomerAccount> allCustomers = new List<CustomerAccount>();
        private static CustomerAccount currentCustomer = null;
        private static List<DateTime> loginHistory = new List<DateTime>();

        private static readonly List<string> greetings = new List<string>
        {
            "Your financial security is our priority.",
            "Welcome to a smarter way to bank.",
            "Have a prosperous day!",
            "Serving you with excellence.",
            "Your trust is our greatest asset."
        };

        private static readonly Random random = new Random();

        private static readonly Dictionary<string, decimal> exchangeRates = new Dictionary<string, decimal>
        {
            {"USD", 1.00m},
            {"CNY", 7.20m},
            {"BDT", 122.37m},
            {"SAR", 3.75m},
            {"EUR", 0.92m}
        };

        static void Main(string[] args)
        {
            Console.Title = "Abdullah Banking Service";
            InitializeSampleData();
            RunBankSystem();
        }

        static void InitializeSampleData()
        {
            allCustomers.Add(new CustomerAccount("ABS-2024-058", "MD Abdullah", "2003", 5500.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-057", "Ajwad Safin", "2257", 3500.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-049", "Rishad Khan", "1234", 4000.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-059", "MD Rohid", "5678", 9999.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-060", "Arnab Mondal", "1760", 2500.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-011", "MD Nasim", "1111", 5500.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-065", "Redoy Hawladar", "6666", 3000.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-010", "Eduerdo", "1013", 70.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-001", "Haochen Wang", "4444", 150000.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-047", "Rudra Thalapati", "8271", 4500.00m));
            allCustomers.Add(new CustomerAccount("ABS-2024-022", "Arko Day", "2222", 5000.00m));
        }

        static void RunBankSystem()
        {
            bool systemRunning = true;

            while (systemRunning)
            {
                ShowMainHeader();

                Console.WriteLine("\n          MAIN MENU");
                Console.WriteLine("   =========================");
                Console.WriteLine("   1. Customer Login");
                Console.WriteLine("   2. Open New Account");
                Console.WriteLine("   3. Banking Tools");
                Console.WriteLine("   4. Bank Information");
                Console.WriteLine("   5. Exit System");
                Console.Write("\n   Select option (1-5): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": CustomerLogin(); break;
                    case "2": CreateNewAccount(); break;
                    case "3": ShowBankingTools(); break;
                    case "4": ShowBankInformation(); break;
                    case "5": systemRunning = false; break;
                    default: Console.WriteLine("   Invalid selection."); Pause(); break;
                }
            }

            Console.WriteLine("\n   Thank you for using Abdullah Banking Service.");
            Console.ReadKey();
        }

        static void ShowMainHeader()
        {
            Console.Clear();
            Console.WriteLine("\n==================================================");
            Console.WriteLine("            ABDULLAH BANKING SERVICE");
            Console.WriteLine("            Secure * Fast * Reliable");
            Console.WriteLine("                 * Since 2024 *");
            Console.WriteLine("==================================================");

            string randomGreeting = greetings[random.Next(greetings.Count)];
            Console.WriteLine($"\n   {randomGreeting}");
            Console.WriteLine($"   Date: {DateTime.Now:dddd, MMMM dd, yyyy}");
            Console.WriteLine($"   Time: {DateTime.Now:HH:mm:ss}");
            Console.WriteLine("--------------------------------------------------");
        }

        static void CustomerLogin()
        {
            ShowMainHeader();
            Console.WriteLine("\n             CUSTOMER LOGIN");
            Console.WriteLine("   ========================================");

            Console.Write("   Account Number: ");
            string accNum = Console.ReadLine();

            Console.Write("   PIN: ");
            string pin = Console.ReadLine();

            currentCustomer = null;
            foreach (CustomerAccount c in allCustomers)
            {
                if (c.AccountNumber == accNum && c.CheckPin(pin))
                {
                    currentCustomer = c;
                    break;
                }
            }

            if (currentCustomer != null)
            {
                currentCustomer.LastLogin = DateTime.Now;
                loginHistory.Add(DateTime.Now);
                Console.WriteLine($"\n   Welcome, {currentCustomer.FullName}!");
                Pause();
                CustomerDashboard();
            }
            else
            {
                Console.WriteLine("   Invalid account or PIN.");
                Pause();
            }
        }

        static void CreateNewAccount()
        {
            ShowMainHeader();
            Console.WriteLine("\n            OPEN NEW ACCOUNT");
            Console.WriteLine("   ========================================");

            Console.Write("   Full name: ");
            string name = Console.ReadLine();

            Console.Write("   4-digit PIN: ");
            string pin = Console.ReadLine();

            Console.Write("   Initial deposit: ");
            string depositInput = Console.ReadLine();

            if (!decimal.TryParse(depositInput, out decimal deposit) || deposit < 0)
            {
                Console.WriteLine("   Invalid amount.");
                Pause();
                return;
            }

            int newNumber = allCustomers.Count + 1;
            string newAccNum = $"ABS-2024-{newNumber:000}";
            CustomerAccount newAccount = new CustomerAccount(newAccNum, name, pin, deposit);
            allCustomers.Add(newAccount);

            Console.WriteLine($"\n   Account created successfully!");
            Console.WriteLine($"   Account Number: {newAccNum}");
            Console.WriteLine($"   Holder: {name}");
            Console.WriteLine($"   Balance: {deposit}");
            Pause();
        }

        static void CustomerDashboard()
        {
            bool inDashboard = true;

            while (inDashboard)
            {
                ShowMainHeader();
                Console.WriteLine($"   Welcome, {currentCustomer.FullName}!");
                Console.WriteLine($"   Account: {currentCustomer.AccountNumber}");
                Console.WriteLine($"   Balance: {currentCustomer.Balance}");
                Console.WriteLine("   ========================================");

                Console.WriteLine("\n   DASHBOARD MENU");
                Console.WriteLine("   1. Deposit Money");
                Console.WriteLine("   2. Withdraw Money");
                Console.WriteLine("   3. Transfer Money");
                Console.WriteLine("   4. Generate Statement");
                Console.WriteLine("   5. Account Details");
                Console.WriteLine("   6. Transaction History");
                Console.WriteLine("   7. Currency Converter");
                Console.WriteLine("   8. Payment Hub");
                Console.WriteLine("   9. Virtual Student Card");
                Console.WriteLine("   10. Security Center");
                Console.WriteLine("   11. Help & Complaint Desk");
                Console.WriteLine("   12. Logout");
                Console.Write("\n   Select option (1-12): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ProcessDeposit(); break;
                    case "2": ProcessWithdrawal(); break;
                    case "3": ProcessTransfer(); break;
                    case "4": currentCustomer.GenerateStatement(); Pause(); break;
                    case "5": currentCustomer.ShowAccountInfo(); Pause(); break;
                    case "6": ViewTransactionHistory(); break;
                    case "7": CurrencyConverter(); break;
                    case "8": PaymentHub(); break;
                    case "9": ShowVirtualCard(); break;
                    case "10": SecurityCenter(); break;
                    case "11": HelpComplaintDesk(); break;
                    case "12": inDashboard = false; currentCustomer = null; break;
                    default: Console.WriteLine("   Invalid selection."); Pause(); break;
                }
            }
        }

        static void ShowVirtualCard()
        {
            Console.Clear();
            Console.WriteLine("\n==================================================");
            Console.WriteLine("           VIRTUAL STUDENT CARD");
            Console.WriteLine("==================================================");
            Console.WriteLine("┌──────────────────────────────────────────┐");
            Console.WriteLine("│   ABDULLAH BANK - VIRTUAL STUDENT CARD   │");
            Console.WriteLine("├──────────────────────────────────────────┤");
            Console.WriteLine($"│  Holder: {currentCustomer.FullName,-27}     │");
            Console.WriteLine($"│  Bank ACC: {currentCustomer.AccountNumber,-22}        │");
            Console.WriteLine($"│  University: {currentCustomer.University,-21}       │");
            Console.WriteLine($"│  Student ID: {currentCustomer.StudentID,-22}      │");
            Console.WriteLine($"│  Valid Until: 2026-12-31                 │");
            Console.WriteLine($"│  Card Status: {(currentCustomer.CardLocked ? "LOCKED" : "ACTIVE"),-22}     │");
            Console.WriteLine("└──────────────────────────────────────────┘");

            Console.WriteLine("\n   Card Functions:");
            Console.WriteLine("   1. Update Student ID");
            Console.WriteLine("   2. Lock/Unlock Card");
            Console.WriteLine("   3. Back to Dashboard");
            Console.Write("\n   Select option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Write("   Enter new Student ID: ");
                    currentCustomer.StudentID = Console.ReadLine();
                    Console.WriteLine("   Student ID updated.");
                    break;
                case "2":
                    if (currentCustomer.CardLocked)
                    {
                        Console.WriteLine("   Card is currently LOCKED");
                        Console.Write("   Unlock card? (Y/N): ");
                        if (Console.ReadLine().ToUpper() == "Y")
                        {
                            currentCustomer.ToggleCardLock(false);
                            Console.WriteLine("   Card unlocked.");
                        }
                    }
                    else
                    {
                        Console.Write("   Lock card for security? (Y/N): ");
                        if (Console.ReadLine().ToUpper() == "Y")
                        {
                            currentCustomer.ToggleCardLock(true);
                            Console.WriteLine("   Card locked.");
                        }
                    }
                    break;
            }

            Pause();
        }

        static void SecurityCenter()
        {
            bool inSecurity = true;

            while (inSecurity)
            {
                Console.Clear();
                Console.WriteLine("\n==================================================");
                Console.WriteLine("              SECURITY CENTER");
                Console.WriteLine("==================================================");
                Console.WriteLine($"   Account: {currentCustomer.AccountNumber}");
                Console.WriteLine($"   Last Login: {currentCustomer.LastLogin:yyyy-MM-dd HH:mm}");
                Console.WriteLine("==================================================");

                Console.WriteLine("\n   1. Change Login PIN");
                Console.WriteLine("   2. Set Security Question");
                Console.WriteLine("   3. Transaction Alerts Settings");
                Console.WriteLine("   4. View Login History");
                Console.WriteLine("   5. Back to Dashboard");
                Console.Write("\n   Select option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("   Current PIN: ");
                        string oldPin = Console.ReadLine();
                        Console.Write("   New PIN (4 digits): ");
                        string newPin = Console.ReadLine();
                        currentCustomer.ChangePin(oldPin, newPin);
                        Console.WriteLine("   PIN changed successfully.");
                        Pause();
                        break;

                    case "2":
                        if (string.IsNullOrEmpty(currentCustomer.SecurityQuestion))
                        {
                            Console.WriteLine("\n   Security Questions:");
                            Console.WriteLine("   1. What is your mother's maiden name?");
                            Console.WriteLine("   2. What city were you born in?");
                            Console.WriteLine("   3. What is your favorite book?");
                            Console.Write("\n   Select question (1-3): ");
                            string qChoice = Console.ReadLine();

                            string question = "";
                            switch (qChoice)
                            {
                                case "1": question = "Mother's maiden name"; break;
                                case "2": question = "Birth city"; break;
                                case "3": question = "Favorite book"; break;
                            }

                            if (!string.IsNullOrEmpty(question))
                            {
                                Console.Write($"   Answer for '{question}': ");
                                string answer = Console.ReadLine();
                                currentCustomer.SetSecurityQuestion(question, answer);
                                Console.WriteLine("   Security question set.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"   Current security question: {currentCustomer.SecurityQuestion}");
                            Console.Write("   Change question? (Y/N): ");
                            if (Console.ReadLine().ToUpper() == "Y")
                            {
                                Console.Write("   New question: ");
                                string newQ = Console.ReadLine();
                                Console.Write("   Answer: ");
                                string newA = Console.ReadLine();
                                currentCustomer.SetSecurityQuestion(newQ, newA);
                                Console.WriteLine("   Security question updated.");
                            }
                        }
                        Pause();
                        break;

                    case "3":
                        Console.WriteLine($"\n   Transaction Alerts: {(currentCustomer.TransactionAlerts ? "ENABLED" : "DISABLED")}");
                        Console.WriteLine("   1. Enable Alerts");
                        Console.WriteLine("   2. Disable Alerts");
                        Console.WriteLine("   3. Back");
                        Console.Write("\n   Select: ");
                        string alertChoice = Console.ReadLine();

                        if (alertChoice == "1")
                        {
                            currentCustomer.ToggleTransactionAlerts(true);
                            Console.WriteLine("   Alerts enabled.");
                        }
                        else if (alertChoice == "2")
                        {
                            currentCustomer.ToggleTransactionAlerts(false);
                            Console.WriteLine("   Alerts disabled.");
                        }
                        Pause();
                        break;

                    case "4":
                        Console.WriteLine("\n   Last 5 Logins:");
                        if (loginHistory.Count == 0)
                        {
                            Console.WriteLine("   No login history.");
                        }
                        else
                        {
                            int start = Math.Max(0, loginHistory.Count - 5);
                            for (int i = start; i < loginHistory.Count; i++)
                            {
                                Console.WriteLine($"   {i + 1}. {loginHistory[i]:yyyy-MM-dd HH:mm:ss}");
                            }
                        }
                        Pause();
                        break;

                    case "5":
                        inSecurity = false;
                        break;
                }
            }
        }

        static void HelpComplaintDesk()
        {
            bool inHelpDesk = true;

            while (inHelpDesk)
            {
                Console.Clear();
                Console.WriteLine("\n==================================================");
                Console.WriteLine("        24/7 HELP & COMPLAINT DESK");
                Console.WriteLine("==================================================");
                Console.WriteLine($"   Account: {currentCustomer.AccountNumber}");
                Console.WriteLine($"   Name: {currentCustomer.FullName}");
                Console.WriteLine("==================================================");

                Console.WriteLine("\n   1. Report Lost/Stolen Card");
                Console.WriteLine("   2. Report Transaction Issue");
                Console.WriteLine("   3. General Inquiry");
                Console.WriteLine("   4. View Complaint Status");
                Console.WriteLine("   5. Emergency Contact");
                Console.WriteLine("   6. Back to Dashboard");
                Console.Write("\n   Select option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        HelpDesk.ReportLostCard(currentCustomer);
                        Pause();
                        break;

                    case "2":
                        HelpDesk.ReportTransactionIssue(currentCustomer);
                        Pause();
                        break;

                    case "3":
                        HelpDesk.SubmitGeneralInquiry(currentCustomer);
                        Pause();
                        break;

                    case "4":
                        HelpDesk.ViewComplaintStatus();
                        Pause();
                        break;

                    case "5":
                        Console.WriteLine("\n   EMERGENCY CONTACTS:");
                        Console.WriteLine("   ---------------------------");
                        Console.WriteLine("   Lost Card Hotline: +86 1772 164 6613");
                        Console.WriteLine("   Fraud Department: +86 5138 501 2794");
                        Console.WriteLine("   Police (PSB): 110");
                        Console.WriteLine("   Ambulance: 120");
                        Console.WriteLine("   Fire: 119");
                        Console.WriteLine("\n   Our Branches (9 AM - 5 PM):");
                        Console.WriteLine("   Nantong University Campus");
                        Console.WriteLine("   Nantong City Center");
                        Console.WriteLine("   Near Railway Station");
                        Pause();
                        break;

                    case "6":
                        inHelpDesk = false;
                        break;
                }
            }
        }

        static void ProcessDeposit()
        {
            ShowMainHeader();
            Console.WriteLine("\n             DEPOSIT MONEY");
            Console.WriteLine("   ========================================");

            Console.Write("   Amount: ");
            string amountInput = Console.ReadLine();

            if (decimal.TryParse(amountInput, out decimal amount))
            {
                currentCustomer.MakeDeposit(amount);
            }
            else
            {
                Console.WriteLine("   Invalid amount.");
            }

            Pause();
        }

        static void ProcessWithdrawal()
        {
            ShowMainHeader();
            Console.WriteLine("\n             WITHDRAW MONEY");
            Console.WriteLine("   ========================================");

            Console.Write("   PIN: ");
            string pin = Console.ReadLine();

            Console.Write("   Amount: ");
            string amountInput = Console.ReadLine();

            if (decimal.TryParse(amountInput, out decimal amount))
            {
                currentCustomer.MakeWithdrawal(amount, pin);
            }
            else
            {
                Console.WriteLine("   Invalid amount.");
            }

            Pause();
        }

        static void ProcessTransfer()
        {
            ShowMainHeader();
            Console.WriteLine("\n              TRANSFER MONEY");
            Console.WriteLine("   ========================================");

            Console.Write("   Recipient account: ");
            string recipientAcc = Console.ReadLine();

            CustomerAccount recipient = null;
            foreach (CustomerAccount c in allCustomers)
            {
                if (c.AccountNumber == recipientAcc)
                {
                    recipient = c;
                    break;
                }
            }

            if (recipient == null || recipient.AccountNumber == currentCustomer.AccountNumber)
            {
                Console.WriteLine("   Invalid recipient.");
                Pause();
                return;
            }

            Console.Write("   Your PIN: ");
            string pin = Console.ReadLine();

            if (!currentCustomer.CheckPin(pin))
            {
                Console.WriteLine("   Invalid PIN.");
                Pause();
                return;
            }

            Console.Write("   Amount: ");
            string amountInput = Console.ReadLine();

            if (!decimal.TryParse(amountInput, out decimal amount) || amount <= 0)
            {
                Console.WriteLine("   Invalid amount.");
                Pause();
                return;
            }

            if (currentCustomer.MakeWithdrawal(amount, pin))
            {
                recipient.ReceiveTransfer(amount, currentCustomer.FullName);
                Console.WriteLine($"   Transferred {amount} to {recipient.FullName}");
            }

            Pause();
        }

        static void ViewTransactionHistory()
        {
            ShowMainHeader();
            Console.WriteLine("\n          TRANSACTION HISTORY");
            Console.WriteLine("   ========================================");

            List<string> history = currentCustomer.GetTransactionHistory();
            if (history.Count == 0)
            {
                Console.WriteLine("   No transactions.");
            }
            else
            {
                foreach (string t in history)
                {
                    Console.WriteLine($"   • {t}");
                }
            }

            Pause();
        }

        static void CurrencyConverter()
        {
            ShowMainHeader();
            Console.WriteLine("\n           CURRENCY CONVERTER");
            Console.WriteLine("   ========================================");

            Console.WriteLine("\n   Currencies: 1.USD 2.CNY 3.BDT 4.SAR 5.EUR");
            Console.Write("   Select base currency (1-5): ");
            string baseChoice = Console.ReadLine();

            string[] codes = { "USD", "CNY", "BDT", "SAR", "EUR" };

            if (int.TryParse(baseChoice, out int idx) && idx >= 1 && idx <= 5)
            {
                string baseCurr = codes[idx - 1];
                Console.Write($"   Amount in {baseCurr}: ");
                string amountInput = Console.ReadLine();

                if (decimal.TryParse(amountInput, out decimal amt) && amt > 0)
                {
                    Console.WriteLine("\n      Conversion Results:");
                    Console.WriteLine("   ----------------------------");

                    foreach (KeyValuePair<string, decimal> curr in exchangeRates)
                    {
                        if (curr.Key != baseCurr)
                        {
                            decimal converted = (amt / exchangeRates[baseCurr]) * curr.Value;
                            Console.WriteLine($"   {amt:N2} {baseCurr} = {converted:N2} {curr.Key}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("   Invalid amount.");
                }
            }
            else
            {
                Console.WriteLine("   Invalid selection.");
            }

            Pause();
        }

        static void ShowBankingTools()
        {
            bool inTools = true;

            while (inTools)
            {
                ShowMainHeader();
                Console.WriteLine("\n              BANKING TOOLS");
                Console.WriteLine("   ========================================");
                Console.WriteLine("   1. Fixed Deposit Calculator");
                Console.WriteLine("   2. Currency Converter");
                Console.WriteLine("   3. Back to Main Menu");
                Console.Write("\n   Select option (1-3): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": FixedDepositCalculator(); break;
                    case "2": CurrencyConverterTool(); break;
                    case "3": inTools = false; break;
                    default: Console.WriteLine("   Invalid."); Pause(); break;
                }
            }
        }

        static void FixedDepositCalculator()
        {
            ShowMainHeader();
            Console.WriteLine("\n         FIXED DEPOSIT CALCULATOR");
            Console.WriteLine("   ========================================");

            Console.Write("   Principal amount: ");
            string principalInput = Console.ReadLine();

            if (!decimal.TryParse(principalInput, out decimal principal) || principal <= 0)
            {
                Console.WriteLine("   Invalid amount.");
                Pause();
                return;
            }

            Console.Write("   Months (1-60): ");
            string monthsInput = Console.ReadLine();

            if (!int.TryParse(monthsInput, out int months) || months < 1 || months > 60)
            {
                Console.WriteLine("   Invalid duration.");
                Pause();
                return;
            }

            decimal rate = months >= 12 ? 0.05m : 0.03m;
            decimal interest = principal * rate * months / 12;
            decimal total = principal + interest;

            Console.WriteLine("\n   CALCULATION RESULTS:");
            Console.WriteLine("   ----------------------------");
            Console.WriteLine($"   Principal: {principal}");
            Console.WriteLine($"   Duration: {months} months");
            Console.WriteLine($"   Rate: {rate:P}");
            Console.WriteLine($"   Interest: {interest}");
            Console.WriteLine($"   Total: {total}");

            Pause();
        }

        static void CurrencyConverterTool()
        {
            ShowMainHeader();
            Console.WriteLine("\n            CURRENCY CONVERTER");
            Console.WriteLine("   ========================================");

            Console.Write("   Amount in USD: ");
            string usdInput = Console.ReadLine();

            if (decimal.TryParse(usdInput, out decimal usd) && usd > 0)
            {
                Console.WriteLine("\n   CONVERSION RESULTS:");
                Console.WriteLine("   ----------------------------");
                Console.WriteLine($"   USD: {usd:N2}");
                Console.WriteLine($"   CNY: {usd * exchangeRates["CNY"]:N2}");
                Console.WriteLine($"   BDT: {usd * exchangeRates["BDT"]:N2}");
                Console.WriteLine($"   SAR: {usd * exchangeRates["SAR"]:N2}");
                Console.WriteLine($"   EUR: {usd * exchangeRates["EUR"]:N2}");
            }
            else
            {
                Console.WriteLine("   Invalid amount.");
            }

            Pause();
        }

        static void PaymentHub()
        {
            bool inPaymentHub = true;

            while (inPaymentHub)
            {
                ShowMainHeader();
                Console.WriteLine("\n               PAYMENT HUB");
                Console.WriteLine("   ========================================");
                Console.WriteLine($"   Account: {currentCustomer.AccountNumber}");
                Console.WriteLine($"   Available Balance: {currentCustomer.Balance}");
                Console.WriteLine("   ========================================\n");

                Console.WriteLine("   PAYMENT CATEGORIES:");
                Console.WriteLine("   1. University Official Payments");
                Console.WriteLine("   2. Government & Visa Services");
                Console.WriteLine("   3. Insurance Services");
                Console.WriteLine("   4. Digital Wallet Top-up");
                Console.WriteLine("   5. Utility Bills");
                Console.WriteLine("   6. Quick Pay");
                Console.WriteLine("   7. View Payment History");
                Console.WriteLine("   8. Back to Dashboard");
                Console.Write("\n   Select option (1-8): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ProcessUniversityPayment(); break;
                    case "2": ProcessGovernmentPayment(); break;
                    case "3": ProcessInsurancePayment(); break;
                    case "4": ProcessDigitalWallet(); break;
                    case "5": ProcessUtilityBills(); break;
                    case "6": QuickPayTemplates(); break;
                    case "7": ViewPaymentHistory(); break;
                    case "8": inPaymentHub = false; break;
                    default: Console.WriteLine("   Invalid selection."); Pause(); break;
                }
            }
        }

        static void ProcessUniversityPayment()
        {
            ShowMainHeader();
            Console.WriteLine("\n        UNIVERSITY PAYMENTS");
            Console.WriteLine("   ========================================");
            Console.WriteLine("\n   Select University Service:");
            Console.WriteLine("   1. Tuition Fees");
            Console.WriteLine("   2. Dormitory Charges");
            Console.WriteLine("   3. Library Fees");
            Console.WriteLine("   4. Exam Registration");
            Console.WriteLine("   5. Custom Amount");
            Console.Write("\n   Select (1-5): ");

            string choice = Console.ReadLine();
            string serviceName = "";
            decimal amount = 0;

            Dictionary<string, decimal> universityFees = new Dictionary<string, decimal>
            {
                {"Tuition Fees", 16000.00m},
                {"Dormitory Charges", 1000.00m},
                {"Library Fees", 300.00m},
                {"Exam Registration", 500.00m}
            };

            if (choice == "5")
            {
                Console.Write("   Enter service name: ");
                serviceName = Console.ReadLine();
                Console.Write("   Enter amount: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.WriteLine("   Invalid amount.");
                    Pause();
                    return;
                }
            }
            else if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= 4)
            {
                string[] services = { "Tuition Fees", "Dormitory Charges", "Library Fees", "Exam Registration" };
                serviceName = services[idx - 1];
                amount = universityFees[serviceName];
            }
            else
            {
                Console.WriteLine("   Invalid selection.");
                Pause();
                return;
            }

            Console.Write($"\n   Confirm payment of {amount} for {serviceName}? (Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.Write("   Enter your PIN: ");
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.WriteLine($"\n   Payment successful!");
                    Console.WriteLine($"   Service: {serviceName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: UNIV-{DateTime.Now:yyyyMMddHHmmss}");
                    GeneratePaymentReceipt(serviceName, amount, "UNIV");
                }
            }
            else
            {
                Console.WriteLine("   Payment cancelled.");
            }
            Pause();
        }

        static void ProcessGovernmentPayment()
        {
            ShowMainHeader();
            Console.WriteLine("\n      GOVERNMENT & VISA SERVICES");
            Console.WriteLine("   ========================================");
            Console.WriteLine("\n   Select Government Service:");
            Console.WriteLine("   1. Visa Extension Fee");
            Console.WriteLine("   2. Residence Permit");
            Console.WriteLine("   3. PSB Registration");
            Console.WriteLine("   4. Notary Services");
            Console.WriteLine("   5. Custom Government Payment");
            Console.Write("\n   Select (1-5): ");

            string choice = Console.ReadLine();
            string serviceName = "";
            decimal amount = 0;

            Dictionary<string, decimal> govtFees = new Dictionary<string, decimal>
            {
                {"Visa Extension Fee", 800.00m},
                {"Residence Permit", 400.00m},
                {"PSB Registration", 200.00m},
                {"Notary Services", 300.00m}
            };

            if (choice == "5")
            {
                Console.Write("   Enter service name: ");
                serviceName = Console.ReadLine();
                Console.Write("   Enter amount: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.WriteLine("   Invalid amount.");
                    Pause();
                    return;
                }
            }
            else if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= 4)
            {
                string[] services = { "Visa Extension Fee", "Residence Permit", "PSB Registration", "Notary Services" };
                serviceName = services[idx - 1];
                amount = govtFees[serviceName];
            }
            else
            {
                Console.WriteLine("   Invalid selection.");
                Pause();
                return;
            }

            Console.Write($"\n   Confirm payment of {amount} for {serviceName}? (Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.Write("   Enter your PIN: ");
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.WriteLine($"\n   Payment successful!");
                    Console.WriteLine($"   Service: {serviceName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: GOV-{DateTime.Now:yyyyMMddHHmmss}");
                    GeneratePaymentReceipt(serviceName, amount, "GOV");
                }
            }
            else
            {
                Console.WriteLine("   Payment cancelled.");
            }
            Pause();
        }

        static void ProcessInsurancePayment()
        {
            ShowMainHeader();
            Console.WriteLine("\n           INSURANCE SERVICES");
            Console.WriteLine("   ========================================");
            Console.WriteLine("\n   Select Insurance Service:");
            Console.WriteLine("   1. Medical Insurance Renewal");
            Console.WriteLine("   2. Travel Insurance");
            Console.WriteLine("   3. Personal Accident Insurance");
            Console.WriteLine("   4. Custom Insurance Payment");
            Console.Write("\n   Select (1-4): ");

            string choice = Console.ReadLine();
            string serviceName = "";
            decimal amount = 0;

            Dictionary<string, decimal> insuranceFees = new Dictionary<string, decimal>
            {
                {"Medical Insurance Renewal", 800.00m},
                {"Travel Insurance", 500.00m},
                {"Personal Accident Insurance", 800.00m}
            };

            if (choice == "4")
            {
                Console.Write("   Enter service name: ");
                serviceName = Console.ReadLine();
                Console.Write("   Enter amount: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.WriteLine("   Invalid amount.");
                    Pause();
                    return;
                }
            }
            else if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= 3)
            {
                string[] services = { "Medical Insurance Renewal", "Travel Insurance", "Personal Accident Insurance" };
                serviceName = services[idx - 1];
                amount = insuranceFees[serviceName];
            }
            else
            {
                Console.WriteLine("   Invalid selection.");
                Pause();
                return;
            }

            Console.Write($"\n   Confirm payment of {amount} for {serviceName}? (Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.Write("   Enter your PIN: ");
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.WriteLine($"\n   Payment successful!");
                    Console.WriteLine($"   Service: {serviceName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: INS-{DateTime.Now:yyyyMMddHHmmss}");
                    GeneratePaymentReceipt(serviceName, amount, "INS");
                }
            }
            else
            {
                Console.WriteLine("   Payment cancelled.");
            }
            Pause();
        }

        static void ProcessDigitalWallet()
        {
            ShowMainHeader();
            Console.WriteLine("\n        DIGITAL WALLET TOP-UP");
            Console.WriteLine("   ========================================");
            Console.WriteLine("\n   Select Wallet:");
            Console.WriteLine("   1. Alipay Balance Top-up");
            Console.WriteLine("   2. WeChat Pay Top-up");
            Console.WriteLine("   3. UnionPay Top-up");
            Console.WriteLine("   4. Custom Wallet Top-up");
            Console.Write("\n   Select (1-4): ");

            string choice = Console.ReadLine();
            string walletName = "";
            decimal amount = 0;

            if (choice == "4")
            {
                Console.Write("   Enter wallet name: ");
                walletName = Console.ReadLine();
                Console.Write("   Enter amount to top-up: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.WriteLine("   Invalid amount.");
                    Pause();
                    return;
                }
            }
            else if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= 3)
            {
                string[] wallets = { "Alipay", "WeChat Pay", "UnionPay" };
                walletName = wallets[idx - 1];

                Console.Write($"   Enter amount to top-up {walletName}: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.WriteLine("   Invalid amount.");
                    Pause();
                    return;
                }
            }
            else
            {
                Console.WriteLine("   Invalid selection.");
                Pause();
                return;
            }

            Console.Write($"\n   Confirm {amount} top-up to {walletName}? (Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.Write("   Enter your PIN: ");
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.WriteLine($"\n   Top-up successful!");
                    Console.WriteLine($"   Wallet: {walletName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: WALLET-{DateTime.Now:yyyyMMddHHmmss}");
                    Console.WriteLine($"   Note: Funds will appear in {walletName} within 5-10 minutes.");
                    GeneratePaymentReceipt($"{walletName} Top-up", amount, "WALLET");
                }
            }
            else
            {
                Console.WriteLine("   Top-up cancelled.");
            }
            Pause();
        }

        static void ProcessUtilityBills()
        {
            ShowMainHeader();
            Console.WriteLine("\n              UTILITY BILLS");
            Console.WriteLine("   ========================================");
            Console.WriteLine("\n   Select Utility:");
            Console.WriteLine("   1. Electricity & Water");
            Console.WriteLine("   2. Internet & Phone");
            Console.WriteLine("   3. Gas Bill");
            Console.WriteLine("   4. Custom Utility Payment");
            Console.Write("\n   Select (1-4): ");

            string choice = Console.ReadLine();
            string utilityName = "";
            decimal amount = 0;

            if (choice == "4")
            {
                Console.Write("   Enter utility name: ");
                utilityName = Console.ReadLine();
                Console.Write("   Enter bill amount: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.WriteLine("   Invalid amount.");
                    Pause();
                    return;
                }
            }
            else if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= 3)
            {
                string[] utilities = { "Electricity & Water", "Internet & Phone", "Gas Bill" };
                utilityName = utilities[idx - 1];

                Console.Write($"   Enter {utilityName} bill amount: ");
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.WriteLine("   Invalid amount.");
                    Pause();
                    return;
                }
            }
            else
            {
                Console.WriteLine("   Invalid selection.");
                Pause();
                return;
            }

            Console.Write($"\n   Confirm payment of {amount} for {utilityName}? (Y/N): ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.Write("   Enter your PIN: ");
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.WriteLine($"\n   Utility payment successful!");
                    Console.WriteLine($"   Utility: {utilityName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: UTIL-{DateTime.Now:yyyyMMddHHmmss}");
                    Console.WriteLine($"   Note: Payment will be processed within 24 hours.");
                    GeneratePaymentReceipt(utilityName + " Bill", amount, "UTIL");
                }
            }
            else
            {
                Console.WriteLine("   Payment cancelled.");
            }
            Pause();
        }

        static void QuickPayTemplates()
        {
            ShowMainHeader();
            Console.WriteLine("\n            QUICK PAY TEMPLATES");
            Console.WriteLine("   ========================================");
            Console.WriteLine("\n   Select Quick Pay Option:");
            Console.WriteLine("   1. Nantong University Tuition - ¥16,000");
            Console.WriteLine("   2. Visa Extension Fee - ¥800");
            Console.WriteLine("   3. Dormitory Monthly - ¥1,000");
            Console.WriteLine("   4. Medical Insurance - ¥800");
            Console.WriteLine("   5. Alipay Top-up (Min) - ¥100");
            Console.WriteLine("   6. Back to Payment Hub");
            Console.Write("\n   Select (1-6): ");

            string choice = Console.ReadLine();

            Dictionary<string, decimal> quickPayOptions = new Dictionary<string, decimal>
            {
                {"Nantong University Tuition", 16000.00m},
                {"Visa Extension Fee", 800.00m},
                {"Dormitory Monthly", 1000.00m},
                {"Medical Insurance", 800.00m},
                {"Alipay Top-up (Min)", 100.00m}
            };

            if (choice == "6")
            {
                return;
            }
            else if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= 5)
            {
                string[] options = {
                    "Nantong University Tuition",
                    "Visa Extension Fee",
                    "Dormitory Monthly",
                    "Medical Insurance",
                    "Alipay Top-up (Min)"
                };

                string serviceName = options[idx - 1];
                decimal amount = quickPayOptions[serviceName];

                Console.Write($"\n   Confirm payment of {amount} for {serviceName}? (Y/N): ");
                if (Console.ReadLine().ToUpper() == "Y")
                {
                    Console.Write("   Enter your PIN: ");
                    string pin = Console.ReadLine();

                    if (currentCustomer.MakeWithdrawal(amount, pin))
                    {
                        Console.WriteLine($"\n   Quick Pay successful!");
                        Console.WriteLine($"   Service: {serviceName}");
                        Console.WriteLine($"   Amount: {amount}");

                        string referencePrefix = "";
                        if (serviceName.Contains("University")) referencePrefix = "UNIV";
                        else if (serviceName.Contains("Visa")) referencePrefix = "GOV";
                        else if (serviceName.Contains("Insurance")) referencePrefix = "INS";
                        else if (serviceName.Contains("Alipay")) referencePrefix = "WALLET";
                        else referencePrefix = "UTIL";

                        Console.WriteLine($"   Reference: {referencePrefix}-Q{DateTime.Now:yyyyMMddHHmmss}");
                        GeneratePaymentReceipt(serviceName, amount, referencePrefix);
                    }
                }
                else
                {
                    Console.WriteLine("   Payment cancelled.");
                }
            }
            else
            {
                Console.WriteLine("   Invalid selection.");
            }
            Pause();
        }

        static void ViewPaymentHistory()
        {
            ShowMainHeader();
            Console.WriteLine("\n          PAYMENT HISTORY");
            Console.WriteLine("   ========================================");

            List<string> history = currentCustomer.GetTransactionHistory();
            var payments = history.FindAll(t => t.Contains("PAYMENT") ||
                                               t.Contains("UNIV") ||
                                               t.Contains("GOV") ||
                                               t.Contains("INS") ||
                                               t.Contains("WALLET") ||
                                               t.Contains("UTIL"));

            if (payments.Count == 0)
            {
                Console.WriteLine("   No payment transactions found.");
            }
            else
            {
                Console.WriteLine("   Recent Payments:");
                Console.WriteLine("   ----------------------------------------");

                foreach (string payment in payments)
                {
                    Console.WriteLine($"   • {payment}");
                }

                Console.WriteLine("\n   Payment Summary:");
                Console.WriteLine("   ----------------------------------------");

                decimal totalPayments = 0;
                foreach (string payment in payments)
                {
                    int start = payment.IndexOf(": -") + 3;
                    int end = payment.IndexOf(" for ");
                    if (start > 0 && end > start)
                    {
                        string amountStr = payment.Substring(start, end - start);
                        if (decimal.TryParse(amountStr, out decimal amount))
                        {
                            totalPayments += amount;
                        }
                    }
                }

                Console.WriteLine($"   Total Payments: {totalPayments}");
                Console.WriteLine($"   Number of Payments: {payments.Count}");
            }

            Pause();
        }

        static void GeneratePaymentReceipt(string serviceName, decimal amount, string category)
        {
            string fileName = $"PaymentReceipt_{category}_{DateTime.Now:yyyyMMddHHmmss}.txt";
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fullPath = Path.Combine(desktopPath, fileName);

            try
            {
                using (StreamWriter writer = new StreamWriter(fullPath))
                {
                    writer.WriteLine("==================================================");
                    writer.WriteLine("            ABDULLAH BANKING SERVICE");
                    writer.WriteLine("               PAYMENT RECEIPT");
                    writer.WriteLine("==================================================");
                    writer.WriteLine($"Customer: {currentCustomer.FullName}");
                    writer.WriteLine($"Account: {currentCustomer.AccountNumber}");
                    writer.WriteLine($"Date: {DateTime.Now:yyyy-MM-dd HH:mm}");
                    writer.WriteLine("==================================================");
                    writer.WriteLine($"Service: {serviceName}");
                    writer.WriteLine($"Amount: {amount}");
                    writer.WriteLine($"Category: {category}");
                    writer.WriteLine($"Reference: {category}-{DateTime.Now:yyyyMMddHHmmss}");
                    writer.WriteLine($"Status: COMPLETED");
                    writer.WriteLine("==================================================");
                    writer.WriteLine("Thank you for using Abdullah Banking Service!");
                    writer.WriteLine("==================================================");
                }
                Console.WriteLine($"   Receipt saved to Desktop: {fileName}");
            }
            catch
            {
                Console.WriteLine("   Could not save receipt.");
            }
        }

        static void ShowBankInformation()
        {
            ShowMainHeader();
            Console.WriteLine("\n                              BANK INFORMATION");
            Console.WriteLine("   =============================================================================");

            Console.WriteLine("\n   About Abdullah Banking Service:");
            Console.WriteLine("   > Founded        : 2024");
            Console.WriteLine("   > Headquarters   : Nantong University, Jiangsu, China");
            Console.WriteLine("   > BANKING HOURS  : Monday- Friday: 9:00 AM - 5:00 PM");
            Console.WriteLine("   > ONLINE BANKING : 24/7 Available");
            Console.WriteLine("   > Specialization : Banking for International Students");
            Console.WriteLine("   > Mission        : Providing secure financial services to students worldwide");

            Console.WriteLine("\n   ## BRANCH LOCATIONS IN NANTONG:");
            Console.WriteLine("   1. Main Branch: Nantong University Campus");
            Console.WriteLine("      Address: 9 Seyuan Road, Chongchuan District, Nantong");

            Console.WriteLine("\n   2. Downtown Branch: Nantong City Center");
            Console.WriteLine("      Address: 188 Renmin Road, Chongchuan District");

            Console.WriteLine("\n   3. Xincheng Branch: Near Nantong Railway Station");
            Console.WriteLine("      Address: 55 Gangzha Road, Gangzha District");

            Console.WriteLine("\n   ## ATM LOCATIONS AT NANTONG UNIVERSITY:");
            Console.WriteLine("   ~ Library Building (24 hours)");
            Console.WriteLine("   ~ Student Center (6 AM - 11 PM)");
            Console.WriteLine("   ~ International Dormitory (24 hours)");
            Console.WriteLine("   ~ Science Building (7 AM - 10 PM)");

            Console.WriteLine("\n   ## INTERNATIONAL SERVICES:");
            Console.WriteLine("   ------------------------------------------------------------------");
            Console.WriteLine("   - Multi-currency accounts (CNY, USD, EUR, BDT, SAR)");
            Console.WriteLine("   - International student loan assistance");
            Console.WriteLine("   - Visa documentation support");
            Console.WriteLine("   - Overseas tuition payment services");

            Console.WriteLine("\n  ## Exchange Rates (per 1 USD):");
            Console.WriteLine("   ------------------------------------------------------------------");
            Console.WriteLine($"     $   CNY: {exchangeRates["CNY"]:N2}");
            Console.WriteLine($"     $   BDT: {exchangeRates["BDT"]:N2}");
            Console.WriteLine($"     $   SAR: {exchangeRates["SAR"]:N2}");
            Console.WriteLine($"     $   EUR: {exchangeRates["EUR"]:N2}");

            Console.WriteLine("\n  ## Contact:");
            Console.WriteLine("   ------------------------------------------------------------------");
            Console.WriteLine("    Customer Service : 3-258-MD ABDULLAH");
            Console.WriteLine("    Email            : support@abdullahbank.com");
            Console.WriteLine("    Website URL      : www.abdullahbank.com/student");
            Console.WriteLine("    WeChat           : AbdullahBank_Support");
            Console.WriteLine("    Phone            : +86 1772 164 6613");
            Console.WriteLine("    Int Helpline     : +86 5138 501 2794");
            Console.WriteLine("    Address          : 9 Seyuan Road, Chongchuan District,Nantong");

            Pause();
        }

        static void Pause()
        {
            Console.Write("\n   Press any key...");
            Console.ReadKey();
        }
    }
}
