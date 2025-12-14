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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid deposit amount.");
                Console.ResetColor();
                return false;
            }

            Balance += amount;
            TransactionHistory.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] DEPOSIT: +{amount}. New Balance: {Balance}");

            if (TransactionAlerts)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   [ALERT]: Deposit of {amount} CNY received at {DateTime.Now:HH:mm}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   Deposited {amount}");
            Console.ResetColor();
            return true;
        }

        public bool MakeWithdrawal(decimal amount, string pin)
        {
            if (!CheckPin(pin))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid PIN.");
                Console.ResetColor();
                return false;
            }

            if (CardLocked)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Card is locked. Please visit branch to unlock.");
                Console.ResetColor();
                return false;
            }

            if (amount <= 0 || amount > 20000)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid withdrawal amount.");
                Console.ResetColor();
                return false;
            }

            if (amount > Balance)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Insufficient funds.");
                Console.ResetColor();
                return false;
            }

            if (amount > Balance * 0.7m)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"   Large withdrawal warning. Confirm? (Y/N): ");
                Console.ResetColor();
                string confirm = Console.ReadLine();
                if (confirm.ToUpper() != "Y")
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("   Cancelled.");
                    Console.ResetColor();
                    return false;
                }
            }

            Balance -= amount;
            TransactionHistory.Add($"[{DateTime.Now:yyyy-MM-dd HH:mm}] WITHDRAWAL: -{amount}. New Balance: {Balance}");

            if (TransactionAlerts)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   [ALERT]: Withdrawal of {amount} CNY made at {DateTime.Now:HH:mm}");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   Withdrew {amount}");
            Console.ResetColor();
            return true;
        }

        public void ShowAccountInfo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n==================================================");
            Console.WriteLine("              ACCOUNT DETAILS");
            Console.WriteLine("==================================================");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"   Account: {AccountNumber}");
            Console.WriteLine($"   Holder: {FullName}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   Balance: {Balance}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"   Student ID: {StudentID}");
            Console.WriteLine($"   University: {University}");

            if (CardLocked)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"   Card Status: {(CardLocked ? "LOCKED" : "Active")}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"   Created: {AccountCreated:yyyy-MM-dd}");
            Console.WriteLine($"   Last Login: {LastLogin:yyyy-MM-dd HH:mm}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("==================================================\n");
            Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   Statement saved to Desktop: {fileName}");
                Console.ResetColor();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Could not save statement.");
                Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   [ALERT]: Transfer of {amount} CNY received from {senderName} at {DateTime.Now:HH:mm}");
                Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n==================================================");
            Console.WriteLine("           REPORT LOST/STOLEN CARD");
            Console.WriteLine("==================================================");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"   Account: {account.AccountNumber}");
            Console.WriteLine($"   Holder: {account.FullName}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   When did you lose it? ");
            Console.ForegroundColor = ConsoleColor.White;
            string when = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   Last location used? ");
            Console.ForegroundColor = ConsoleColor.White;
            string location = Console.ReadLine();

            account.ToggleCardLock(true);

            string ticket = $"LOST-CARD-{DateTime.Now:yyyyMMddHHmmss}";
            string complaint = $"[{DateTime.Now}] Ticket: {ticket} | Account: {account.AccountNumber} | Type: Lost Card | When: {when} | Location: {location}";
            complaints.Add(complaint);

            SaveComplaintToFile(complaint);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n   ✓ Card locked for security");
            Console.WriteLine($"   ✓ Ticket created: {ticket}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   ✓ Visit any branch with your passport to get replacement");
            Console.WriteLine("   ✓ Contact: +86 1772 164 6613 (24/7 Lost Card Hotline)");
            Console.ResetColor();
        }

        public static void ReportTransactionIssue(CustomerAccount account)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n==================================================");
            Console.WriteLine("           REPORT TRANSACTION ISSUE");
            Console.WriteLine("==================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   Transaction Date (YYYY-MM-DD): ");
            Console.ForegroundColor = ConsoleColor.White;
            string date = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   Transaction Amount: ");
            Console.ForegroundColor = ConsoleColor.White;
            string amount = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   Issue Description: ");
            Console.ForegroundColor = ConsoleColor.White;
            string description = Console.ReadLine();

            string ticket = $"TRX-{DateTime.Now:yyyyMMddHHmmss}";
            string complaint = $"[{DateTime.Now}] Ticket: {ticket} | Account: {account.AccountNumber} | Type: Transaction Issue | Date: {date} | Amount: {amount} | Issue: {description}";
            complaints.Add(complaint);

            SaveComplaintToFile(complaint);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n   ✓ Ticket created: {ticket}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   ✓ Our team will investigate within 2 business days");
            Console.WriteLine("   ✓ You will receive SMS update");
            Console.ResetColor();
        }

        public static void SubmitGeneralInquiry(CustomerAccount account)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n==================================================");
            Console.WriteLine("              GENERAL INQUIRY");
            Console.WriteLine("==================================================");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   1. Account Opening Requirements");
            Console.WriteLine("   2. International Transfer Help");
            Console.WriteLine("   3. PSB Registration Support");
            Console.WriteLine("   4. Student Loan Information");
            Console.WriteLine("   5. Other");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\n   Select inquiry type: ");
            Console.ForegroundColor = ConsoleColor.White;
            string type = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   Your message: ");
            Console.ForegroundColor = ConsoleColor.White;
            string message = Console.ReadLine();

            string ticket = $"INQ-{DateTime.Now:yyyyMMddHHmmss}";
            string complaint = $"[{DateTime.Now}] Ticket: {ticket} | Account: {account.AccountNumber} | Type: Inquiry-{type} | Message: {message}";
            complaints.Add(complaint);

            SaveComplaintToFile(complaint);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n   ✓ Ticket created: {ticket}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("   ✓ We'll respond within 24 hours via email");
            Console.WriteLine("   ✓ Check your registered email for updates");
            Console.ResetColor();
        }

        public static void ViewComplaintStatus()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n==================================================");
            Console.WriteLine("           COMPLAINT STATUS");
            Console.WriteLine("==================================================");
            Console.ForegroundColor = ConsoleColor.White;

            if (complaints.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("   No complaints submitted.");
                Console.ResetColor();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var complaint in complaints)
            {
                Console.WriteLine($"   • {complaint}");
            }
            Console.ResetColor();
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

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n          MAIN MENU");
                Console.WriteLine("   =========================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("   1. Customer Login");
                Console.WriteLine("   2. Open New Account");
                Console.WriteLine("   3. Banking Tools");
                Console.WriteLine("   4. Bank Information");
                Console.WriteLine("   5. Exit System");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n   Select option (1-5): ");
                Console.ResetColor();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": CustomerLogin(); break;
                    case "2": CreateNewAccount(); break;
                    case "3": ShowBankingTools(); break;
                    case "4": ShowBankInformation(); break;
                    case "5": systemRunning = false; break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Invalid selection.");
                        Console.ResetColor();
                        Pause();
                        break;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n   Thank you for using Abdullah Banking Service.");
            Console.ResetColor();
            Console.ReadKey();
        }

        static void ShowMainHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n==================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("            ABDULLAH BANKING SERVICE");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("            Secure * Fast * Reliable");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("                 * Since 2024 *");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("==================================================");
            Console.ResetColor();

            string randomGreeting = greetings[random.Next(greetings.Count)];
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n   {randomGreeting}");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"   Date: {DateTime.Now:dddd, MMMM dd, yyyy}");
            Console.WriteLine($"   Time: {DateTime.Now:HH:mm:ss}");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("--------------------------------------------------");
            Console.ResetColor();
        }

        static void CustomerLogin()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n             CUSTOMER LOGIN");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Account Number: ");
            Console.ForegroundColor = ConsoleColor.White;
            string accNum = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   PIN: ");
            Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n   Welcome, {currentCustomer.FullName}!");
                Console.ResetColor();
                Pause();
                CustomerDashboard();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid account or PIN.");
                Console.ResetColor();
                Pause();
            }
        }

        static void CreateNewAccount()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n            OPEN NEW ACCOUNT");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Full name: ");
            Console.ForegroundColor = ConsoleColor.White;
            string name = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   4-digit PIN: ");
            Console.ForegroundColor = ConsoleColor.White;
            string pin = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Initial deposit: ");
            Console.ForegroundColor = ConsoleColor.White;
            string depositInput = Console.ReadLine();

            if (!decimal.TryParse(depositInput, out decimal deposit) || deposit < 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid amount.");
                Console.ResetColor();
                Pause();
                return;
            }

            int newNumber = allCustomers.Count + 1;
            string newAccNum = $"ABS-2024-{newNumber:000}";
            CustomerAccount newAccount = new CustomerAccount(newAccNum, name, pin, deposit);
            allCustomers.Add(newAccount);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n   Account created successfully!");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"   Account Number: {newAccNum}");
            Console.WriteLine($"   Holder: {name}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   Balance: {deposit}");
            Console.ResetColor();
            Pause();
        }

        static void CustomerDashboard()
        {
            bool inDashboard = true;

            while (inDashboard)
            {
                ShowMainHeader();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   Welcome, {currentCustomer.FullName}!");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"   Account: {currentCustomer.AccountNumber}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   Balance: {currentCustomer.Balance}");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("   ========================================");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n   DASHBOARD MENU");
                Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n   Select option (1-12): ");
                Console.ResetColor();

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
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Invalid selection.");
                        Console.ResetColor();
                        Pause();
                        break;
                }
            }
        }

        static void ShowVirtualCard()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n==================================================");
            Console.WriteLine("           VIRTUAL STUDENT CARD");
            Console.WriteLine("==================================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("┌──────────────────────────────────────────┐");
            Console.WriteLine("│   ABDULLAH BANK - VIRTUAL STUDENT CARD   │");
            Console.WriteLine("├──────────────────────────────────────────┤");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"│  Holder: {currentCustomer.FullName,-27}     │");
            Console.WriteLine($"│  Bank ACC: {currentCustomer.AccountNumber,-22}        │");
            Console.WriteLine($"│  University: {currentCustomer.University,-21}       │");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"│  Student ID: {currentCustomer.StudentID,-22}      │");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"│  Valid Until: 2026-12-31                 │");

            if (currentCustomer.CardLocked)
                Console.ForegroundColor = ConsoleColor.Red;
            else
                Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine($"│  Card Status: {(currentCustomer.CardLocked ? "LOCKED" : "ACTIVE"),-22}     │");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("└──────────────────────────────────────────┘");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   Card Functions:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   1. Update Student ID");
            Console.WriteLine("   2. Lock/Unlock Card");
            Console.WriteLine("   3. Back to Dashboard");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n   Select option: ");
            Console.ResetColor();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("   Enter new Student ID: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    currentCustomer.StudentID = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("   Student ID updated.");
                    Console.ResetColor();
                    break;
                case "2":
                    if (currentCustomer.CardLocked)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Card is currently LOCKED");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("   Unlock card? (Y/N): ");
                        Console.ForegroundColor = ConsoleColor.White;
                        if (Console.ReadLine().ToUpper() == "Y")
                        {
                            currentCustomer.ToggleCardLock(false);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("   Card unlocked.");
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("   Lock card for security? (Y/N): ");
                        Console.ForegroundColor = ConsoleColor.White;
                        if (Console.ReadLine().ToUpper() == "Y")
                        {
                            currentCustomer.ToggleCardLock(true);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("   Card locked.");
                            Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("\n==================================================");
                Console.WriteLine("              SECURITY CENTER");
                Console.WriteLine("==================================================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"   Account: {currentCustomer.AccountNumber}");
                Console.WriteLine($"   Last Login: {currentCustomer.LastLogin:yyyy-MM-dd HH:mm}");
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("==================================================");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n   1. Change Login PIN");
                Console.WriteLine("   2. Set Security Question");
                Console.WriteLine("   3. Transaction Alerts Settings");
                Console.WriteLine("   4. View Login History");
                Console.WriteLine("   5. Back to Dashboard");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n   Select option: ");
                Console.ResetColor();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("   Current PIN: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        string oldPin = Console.ReadLine();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("   New PIN (4 digits): ");
                        Console.ForegroundColor = ConsoleColor.White;
                        string newPin = Console.ReadLine();
                        currentCustomer.ChangePin(oldPin, newPin);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("   PIN changed successfully.");
                        Console.ResetColor();
                        Pause();
                        break;

                    case "2":
                        if (string.IsNullOrEmpty(currentCustomer.SecurityQuestion))
                        {
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("\n   Security Questions:");
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine("   1. What is your mother's maiden name?");
                            Console.WriteLine("   2. What city were you born in?");
                            Console.WriteLine("   3. What is your favorite book?");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("\n   Select question (1-3): ");
                            Console.ForegroundColor = ConsoleColor.White;
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
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"   Answer for '{question}': ");
                                Console.ForegroundColor = ConsoleColor.White;
                                string answer = Console.ReadLine();
                                currentCustomer.SetSecurityQuestion(question, answer);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("   Security question set.");
                                Console.ResetColor();
                            }
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"   Current security question: {currentCustomer.SecurityQuestion}");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.Write("   Change question? (Y/N): ");
                            Console.ForegroundColor = ConsoleColor.White;
                            if (Console.ReadLine().ToUpper() == "Y")
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("   New question: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                string newQ = Console.ReadLine();
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write("   Answer: ");
                                Console.ForegroundColor = ConsoleColor.White;
                                string newA = Console.ReadLine();
                                currentCustomer.SetSecurityQuestion(newQ, newA);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("   Security question updated.");
                                Console.ResetColor();
                            }
                        }
                        Pause();
                        break;

                    case "3":
                        if (currentCustomer.TransactionAlerts)
                            Console.ForegroundColor = ConsoleColor.Green;
                        else
                            Console.ForegroundColor = ConsoleColor.Red;

                        Console.WriteLine($"\n   Transaction Alerts: {(currentCustomer.TransactionAlerts ? "ENABLED" : "DISABLED")}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("   1. Enable Alerts");
                        Console.WriteLine("   2. Disable Alerts");
                        Console.WriteLine("   3. Back");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("\n   Select: ");
                        Console.ForegroundColor = ConsoleColor.White;
                        string alertChoice = Console.ReadLine();

                        if (alertChoice == "1")
                        {
                            currentCustomer.ToggleTransactionAlerts(true);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("   Alerts enabled.");
                            Console.ResetColor();
                        }
                        else if (alertChoice == "2")
                        {
                            currentCustomer.ToggleTransactionAlerts(false);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("   Alerts disabled.");
                            Console.ResetColor();
                        }
                        Pause();
                        break;

                    case "4":
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\n   Last 5 Logins:");
                        Console.ForegroundColor = ConsoleColor.White;

                        if (loginHistory.Count == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine("   No login history.");
                            Console.ResetColor();
                        }
                        else
                        {
                            int start = Math.Max(0, loginHistory.Count - 5);
                            for (int i = start; i < loginHistory.Count; i++)
                            {
                                Console.WriteLine($"   {i + 1}. {loginHistory[i]:yyyy-MM-dd HH:mm:ss}");
                            }
                        }
                        Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("\n==================================================");
                Console.WriteLine("        24/7 HELP & COMPLAINT DESK");
                Console.WriteLine("==================================================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"   Account: {currentCustomer.AccountNumber}");
                Console.WriteLine($"   Name: {currentCustomer.FullName}");
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("==================================================");
                Console.ResetColor();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n   1. Report Lost/Stolen Card");
                Console.WriteLine("   2. Report Transaction Issue");
                Console.WriteLine("   3. General Inquiry");
                Console.WriteLine("   4. View Complaint Status");
                Console.WriteLine("   5. Emergency Contact");
                Console.WriteLine("   6. Back to Dashboard");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n   Select option: ");
                Console.ResetColor();

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
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\n   EMERGENCY CONTACTS:");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine("   ---------------------------");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("   Lost Card Hotline: +86 1772 164 6613");
                        Console.WriteLine("   Fraud Department: +86 5138 501 2794");
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Police (PSB): 110");
                        Console.WriteLine("   Ambulance: 120");
                        Console.WriteLine("   Fire: 119");
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\n   Our Branches (9 AM - 5 PM):");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("   Nantong University Campus");
                        Console.WriteLine("   Nantong City Center");
                        Console.WriteLine("   Near Railway Station");
                        Console.ResetColor();
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n             DEPOSIT MONEY");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Amount: ");
            Console.ForegroundColor = ConsoleColor.White;
            string amountInput = Console.ReadLine();

            if (decimal.TryParse(amountInput, out decimal amount))
            {
                currentCustomer.MakeDeposit(amount);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid amount.");
                Console.ResetColor();
            }

            Pause();
        }

        static void ProcessWithdrawal()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n             WITHDRAW MONEY");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   PIN: ");
            Console.ForegroundColor = ConsoleColor.White;
            string pin = Console.ReadLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("   Amount: ");
            Console.ForegroundColor = ConsoleColor.White;
            string amountInput = Console.ReadLine();

            if (decimal.TryParse(amountInput, out decimal amount))
            {
                currentCustomer.MakeWithdrawal(amount, pin);
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid amount.");
                Console.ResetColor();
            }

            Pause();
        }

        static void ProcessTransfer()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n              TRANSFER MONEY");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Recipient account: ");
            Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid recipient.");
                Console.ResetColor();
                Pause();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Your PIN: ");
            Console.ForegroundColor = ConsoleColor.White;
            string pin = Console.ReadLine();

            if (!currentCustomer.CheckPin(pin))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid PIN.");
                Console.ResetColor();
                Pause();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Amount: ");
            Console.ForegroundColor = ConsoleColor.White;
            string amountInput = Console.ReadLine();

            if (!decimal.TryParse(amountInput, out decimal amount) || amount <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid amount.");
                Console.ResetColor();
                Pause();
                return;
            }

            if (currentCustomer.MakeWithdrawal(amount, pin))
            {
                recipient.ReceiveTransfer(amount, currentCustomer.FullName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   Transferred {amount} to {recipient.FullName}");
                Console.ResetColor();
            }

            Pause();
        }

        static void ViewTransactionHistory()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n          TRANSACTION HISTORY");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.White;

            List<string> history = currentCustomer.GetTransactionHistory();
            if (history.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("   No transactions.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (string t in history)
                {
                    Console.WriteLine($"   • {t}");
                }
                Console.ResetColor();
            }

            Pause();
        }

        static void CurrencyConverter()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n           CURRENCY CONVERTER");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   Currencies: 1.USD 2.CNY 3.BDT 4.SAR 5.EUR");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Select base currency (1-5): ");
            Console.ForegroundColor = ConsoleColor.White;
            string baseChoice = Console.ReadLine();

            string[] codes = { "USD", "CNY", "BDT", "SAR", "EUR" };

            if (int.TryParse(baseChoice, out int idx) && idx >= 1 && idx <= 5)
            {
                string baseCurr = codes[idx - 1];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"   Amount in {baseCurr}: ");
                Console.ForegroundColor = ConsoleColor.White;
                string amountInput = Console.ReadLine();

                if (decimal.TryParse(amountInput, out decimal amt) && amt > 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\n      Conversion Results:");
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("   ----------------------------");
                    Console.ForegroundColor = ConsoleColor.White;

                    foreach (KeyValuePair<string, decimal> curr in exchangeRates)
                    {
                        if (curr.Key != baseCurr)
                        {
                            decimal converted = (amt / exchangeRates[baseCurr]) * curr.Value;
                            Console.WriteLine($"   {amt:N2} {baseCurr} = {converted:N2} {curr.Key}");
                        }
                    }
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   Invalid amount.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid selection.");
                Console.ResetColor();
            }

            Pause();
        }

        static void ShowBankingTools()
        {
            bool inTools = true;

            while (inTools)
            {
                ShowMainHeader();
                Console.ForegroundColor = ConsoleColor.DarkCyan;
                Console.WriteLine("\n              BANKING TOOLS");
                Console.WriteLine("   ========================================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("   1. Fixed Deposit Calculator");
                Console.WriteLine("   2. Currency Converter");
                Console.WriteLine("   3. Back to Main Menu");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n   Select option (1-3): ");
                Console.ResetColor();

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": FixedDepositCalculator(); break;
                    case "2": CurrencyConverterTool(); break;
                    case "3": inTools = false; break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Invalid.");
                        Console.ResetColor();
                        Pause();
                        break;
                }
            }
        }

        static void FixedDepositCalculator()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n         FIXED DEPOSIT CALCULATOR");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Principal amount: ");
            Console.ForegroundColor = ConsoleColor.White;
            string principalInput = Console.ReadLine();

            if (!decimal.TryParse(principalInput, out decimal principal) || principal <= 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid amount.");
                Console.ResetColor();
                Pause();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Months (1-60): ");
            Console.ForegroundColor = ConsoleColor.White;
            string monthsInput = Console.ReadLine();

            if (!int.TryParse(monthsInput, out int months) || months < 1 || months > 60)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid duration.");
                Console.ResetColor();
                Pause();
                return;
            }

            decimal rate = months >= 12 ? 0.05m : 0.03m;
            decimal interest = principal * rate * months / 12;
            decimal total = principal + interest;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   CALCULATION RESULTS:");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("   ----------------------------");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"   Principal: {principal}");
            Console.WriteLine($"   Duration: {months} months");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"   Rate: {rate:P}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"   Interest: {interest}");
            Console.WriteLine($"   Total: {total}");
            Console.ResetColor();

            Pause();
        }

        static void CurrencyConverterTool()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\n            CURRENCY CONVERTER");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("   Amount in USD: ");
            Console.ForegroundColor = ConsoleColor.White;
            string usdInput = Console.ReadLine();

            if (decimal.TryParse(usdInput, out decimal usd) && usd > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\n   CONVERSION RESULTS:");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("   ----------------------------");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"   USD: {usd:N2}");
                Console.WriteLine($"   CNY: {usd * exchangeRates["CNY"]:N2}");
                Console.WriteLine($"   BDT: {usd * exchangeRates["BDT"]:N2}");
                Console.WriteLine($"   SAR: {usd * exchangeRates["SAR"]:N2}");
                Console.WriteLine($"   EUR: {usd * exchangeRates["EUR"]:N2}");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid amount.");
                Console.ResetColor();
            }

            Pause();
        }

        static void PaymentHub()
        {
            bool inPaymentHub = true;

            while (inPaymentHub)
            {
                ShowMainHeader();
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("\n               PAYMENT HUB");
                Console.WriteLine("   ========================================");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"   Account: {currentCustomer.AccountNumber}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   Available Balance: {currentCustomer.Balance}");
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("   ========================================\n");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("   PAYMENT CATEGORIES:");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("   1. University Official Payments");
                Console.WriteLine("   2. Government & Visa Services");
                Console.WriteLine("   3. Insurance Services");
                Console.WriteLine("   4. Digital Wallet Top-up");
                Console.WriteLine("   5. Utility Bills");
                Console.WriteLine("   6. Quick Pay");
                Console.WriteLine("   7. View Payment History");
                Console.WriteLine("   8. Back to Dashboard");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("\n   Select option (1-8): ");
                Console.ResetColor();

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
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("   Invalid selection.");
                        Console.ResetColor();
                        Pause();
                        break;
                }
            }
        }

        static void ProcessUniversityPayment()
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("\n        UNIVERSITY PAYMENTS");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   Select University Service:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   1. Tuition Fees");
            Console.WriteLine("   2. Dormitory Charges");
            Console.WriteLine("   3. Library Fees");
            Console.WriteLine("   4. Exam Registration");
            Console.WriteLine("   5. Custom Amount");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n   Select (1-5): ");
            Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter service name: ");
                Console.ForegroundColor = ConsoleColor.White;
                serviceName = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter amount: ");
                Console.ForegroundColor = ConsoleColor.White;
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   Invalid amount.");
                    Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid selection.");
                Console.ResetColor();
                Pause();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\n   Confirm payment of {amount} for {serviceName}? (Y/N): ");
            Console.ForegroundColor = ConsoleColor.White;
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter your PIN: ");
                Console.ForegroundColor = ConsoleColor.White;
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n   Payment successful!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"   Service: {serviceName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: UNIV-{DateTime.Now:yyyyMMddHHmmss}");
                    Console.ResetColor();
                    GeneratePaymentReceipt(serviceName, amount, "UNIV");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("   Payment cancelled.");
                Console.ResetColor();
            }
            Pause();
        }

        static void ProcessGovernmentPayment()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\n      GOVERNMENT & VISA SERVICES");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   Select Government Service:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   1. Visa Extension Fee");
            Console.WriteLine("   2. Residence Permit");
            Console.WriteLine("   3. PSB Registration");
            Console.WriteLine("   4. Notary Services");
            Console.WriteLine("   5. Custom Government Payment");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n   Select (1-5): ");
            Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter service name: ");
                Console.ForegroundColor = ConsoleColor.White;
                serviceName = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter amount: ");
                Console.ForegroundColor = ConsoleColor.White;
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   Invalid amount.");
                    Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid selection.");
                Console.ResetColor();
                Pause();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\n   Confirm payment of {amount} for {serviceName}? (Y/N): ");
            Console.ForegroundColor = ConsoleColor.White;
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter your PIN: ");
                Console.ForegroundColor = ConsoleColor.White;
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n   Payment successful!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"   Service: {serviceName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: GOV-{DateTime.Now:yyyyMMddHHmmss}");
                    Console.ResetColor();
                    GeneratePaymentReceipt(serviceName, amount, "GOV");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("   Payment cancelled.");
                Console.ResetColor();
            }
            Pause();
        }

        static void ProcessInsurancePayment()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("\n           INSURANCE SERVICES");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   Select Insurance Service:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   1. Medical Insurance Renewal");
            Console.WriteLine("   2. Travel Insurance");
            Console.WriteLine("   3. Personal Accident Insurance");
            Console.WriteLine("   4. Custom Insurance Payment");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n   Select (1-4): ");
            Console.ForegroundColor = ConsoleColor.White;
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
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter service name: ");
                Console.ForegroundColor = ConsoleColor.White;
                serviceName = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter amount: ");
                Console.ForegroundColor = ConsoleColor.White;
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   Invalid amount.");
                    Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid selection.");
                Console.ResetColor();
                Pause();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\n   Confirm payment of {amount} for {serviceName}? (Y/N): ");
            Console.ForegroundColor = ConsoleColor.White;
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter your PIN: ");
                Console.ForegroundColor = ConsoleColor.White;
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n   Payment successful!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"   Service: {serviceName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: INS-{DateTime.Now:yyyyMMddHHmmss}");
                    Console.ResetColor();
                    GeneratePaymentReceipt(serviceName, amount, "INS");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("   Payment cancelled.");
                Console.ResetColor();
            }
            Pause();
        }

        static void ProcessDigitalWallet()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("\n        DIGITAL WALLET TOP-UP");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   Select Wallet:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   1. Alipay Balance Top-up");
            Console.WriteLine("   2. WeChat Pay Top-up");
            Console.WriteLine("   3. UnionPay Top-up");
            Console.WriteLine("   4. Custom Wallet Top-up");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n   Select (1-4): ");
            Console.ForegroundColor = ConsoleColor.White;
            string choice = Console.ReadLine();
            string walletName = "";
            decimal amount = 0;

            if (choice == "4")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter wallet name: ");
                Console.ForegroundColor = ConsoleColor.White;
                walletName = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter amount to top-up: ");
                Console.ForegroundColor = ConsoleColor.White;
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   Invalid amount.");
                    Console.ResetColor();
                    Pause();
                    return;
                }
            }
            else if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= 3)
            {
                string[] wallets = { "Alipay", "WeChat Pay", "UnionPay" };
                walletName = wallets[idx - 1];

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"   Enter amount to top-up {walletName}: ");
                Console.ForegroundColor = ConsoleColor.White;
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   Invalid amount.");
                    Console.ResetColor();
                    Pause();
                    return;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid selection.");
                Console.ResetColor();
                Pause();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\n   Confirm {amount} top-up to {walletName}? (Y/N): ");
            Console.ForegroundColor = ConsoleColor.White;
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter your PIN: ");
                Console.ForegroundColor = ConsoleColor.White;
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n   Top-up successful!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"   Wallet: {walletName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: WALLET-{DateTime.Now:yyyyMMddHHmmss}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"   Note: Funds will appear in {walletName} within 5-10 minutes.");
                    Console.ResetColor();
                    GeneratePaymentReceipt($"{walletName} Top-up", amount, "WALLET");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("   Top-up cancelled.");
                Console.ResetColor();
            }
            Pause();
        }

        static void ProcessUtilityBills()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n              UTILITY BILLS");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   Select Utility:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   1. Electricity & Water");
            Console.WriteLine("   2. Internet & Phone");
            Console.WriteLine("   3. Gas Bill");
            Console.WriteLine("   4. Custom Utility Payment");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n   Select (1-4): ");
            Console.ForegroundColor = ConsoleColor.White;
            string choice = Console.ReadLine();
            string utilityName = "";
            decimal amount = 0;

            if (choice == "4")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter utility name: ");
                Console.ForegroundColor = ConsoleColor.White;
                utilityName = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter bill amount: ");
                Console.ForegroundColor = ConsoleColor.White;
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   Invalid amount.");
                    Console.ResetColor();
                    Pause();
                    return;
                }
            }
            else if (int.TryParse(choice, out int idx) && idx >= 1 && idx <= 3)
            {
                string[] utilities = { "Electricity & Water", "Internet & Phone", "Gas Bill" };
                utilityName = utilities[idx - 1];

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"   Enter {utilityName} bill amount: ");
                Console.ForegroundColor = ConsoleColor.White;
                if (!decimal.TryParse(Console.ReadLine(), out amount) || amount <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("   Invalid amount.");
                    Console.ResetColor();
                    Pause();
                    return;
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid selection.");
                Console.ResetColor();
                Pause();
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"\n   Confirm payment of {amount} for {utilityName}? (Y/N): ");
            Console.ForegroundColor = ConsoleColor.White;
            if (Console.ReadLine().ToUpper() == "Y")
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("   Enter your PIN: ");
                Console.ForegroundColor = ConsoleColor.White;
                string pin = Console.ReadLine();

                if (currentCustomer.MakeWithdrawal(amount, pin))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n   Utility payment successful!");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"   Utility: {utilityName}");
                    Console.WriteLine($"   Amount: {amount}");
                    Console.WriteLine($"   Reference: UTIL-{DateTime.Now:yyyyMMddHHmmss}");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"   Note: Payment will be processed within 24 hours.");
                    Console.ResetColor();
                    GeneratePaymentReceipt(utilityName + " Bill", amount, "UTIL");
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("   Payment cancelled.");
                Console.ResetColor();
            }
            Pause();
        }

        static void QuickPayTemplates()
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n            QUICK PAY TEMPLATES");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   Select Quick Pay Option:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   1. Nantong University Tuition - ¥16,000");
            Console.WriteLine("   2. Visa Extension Fee - ¥800");
            Console.WriteLine("   3. Dormitory Monthly - ¥1,000");
            Console.WriteLine("   4. Medical Insurance - ¥800");
            Console.WriteLine("   5. Alipay Top-up (Min) - ¥100");
            Console.WriteLine("   6. Back to Payment Hub");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\n   Select (1-6): ");
            Console.ForegroundColor = ConsoleColor.White;
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

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"\n   Confirm payment of {amount} for {serviceName}? (Y/N): ");
                Console.ForegroundColor = ConsoleColor.White;
                if (Console.ReadLine().ToUpper() == "Y")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write("   Enter your PIN: ");
                    Console.ForegroundColor = ConsoleColor.White;
                    string pin = Console.ReadLine();

                    if (currentCustomer.MakeWithdrawal(amount, pin))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\n   Quick Pay successful!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"   Service: {serviceName}");
                        Console.WriteLine($"   Amount: {amount}");

                        string referencePrefix = "";
                        if (serviceName.Contains("University")) referencePrefix = "UNIV";
                        else if (serviceName.Contains("Visa")) referencePrefix = "GOV";
                        else if (serviceName.Contains("Insurance")) referencePrefix = "INS";
                        else if (serviceName.Contains("Alipay")) referencePrefix = "WALLET";
                        else referencePrefix = "UTIL";

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"   Reference: {referencePrefix}-Q{DateTime.Now:yyyyMMddHHmmss}");
                        Console.ResetColor();
                        GeneratePaymentReceipt(serviceName, amount, referencePrefix);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("   Payment cancelled.");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Invalid selection.");
                Console.ResetColor();
            }
            Pause();
        }

        static void ViewPaymentHistory()
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("\n          PAYMENT HISTORY");
            Console.WriteLine("   ========================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("   Recent Payments:");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("   ----------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;

            List<string> history = currentCustomer.GetTransactionHistory();
            var payments = history.FindAll(t => t.Contains("PAYMENT") ||
                                               t.Contains("UNIV") ||
                                               t.Contains("GOV") ||
                                               t.Contains("INS") ||
                                               t.Contains("WALLET") ||
                                               t.Contains("UTIL"));

            if (payments.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("   No payment transactions found.");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                foreach (string payment in payments)
                {
                    Console.WriteLine($"   • {payment}");
                }
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n   Payment Summary:");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("   ----------------------------------------");
                Console.ForegroundColor = ConsoleColor.White;

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

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   Total Payments: {totalPayments}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"   Number of Payments: {payments.Count}");
                Console.ResetColor();
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
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"   Receipt saved to Desktop: {fileName}");
                Console.ResetColor();
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("   Could not save receipt.");
                Console.ResetColor();
            }
        }

        static void ShowBankInformation()
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("\n                              BANK INFORMATION");
            Console.WriteLine("   =============================================================================");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n   About Abdullah Banking Service:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   > Founded        : 2024");
            Console.WriteLine("   > Headquarters   : Nantong University, Jiangsu, China");
            Console.WriteLine("   > BANKING HOURS  : Monday- Friday: 9:00 AM - 5:00 PM");
            Console.WriteLine("   > ONLINE BANKING : 24/7 Available");
            Console.WriteLine("   > Specialization : Banking for International Students");
            Console.WriteLine("   > Mission        : Providing secure financial services to students worldwide");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n   ## BRANCH LOCATIONS IN NANTONG:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   1. Main Branch: Nantong University Campus");
            Console.WriteLine("      Address: 9 Seyuan Road, Chongchuan District, Nantong");
            Console.WriteLine("\n   2. Downtown Branch: Nantong City Center");
            Console.WriteLine("      Address: 188 Renmin Road, Chongchuan District");
            Console.WriteLine("\n   3. Xincheng Branch: Near Nantong Railway Station");
            Console.WriteLine("      Address: 55 Gangzha Road, Gangzha District");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n   ## ATM LOCATIONS AT NANTONG UNIVERSITY:");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   ~ Library Building (24 hours)");
            Console.WriteLine("   ~ Student Center (6 AM - 11 PM)");
            Console.WriteLine("   ~ International Dormitory (24 hours)");
            Console.WriteLine("   ~ Science Building (7 AM - 10 PM)");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n   ## INTERNATIONAL SERVICES:");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("   ------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("   - Multi-currency accounts (CNY, USD, EUR, BDT, SAR)");
            Console.WriteLine("   - International student loan assistance");
            Console.WriteLine("   - Visa documentation support");
            Console.WriteLine("   - Overseas tuition payment services");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  ## Exchange Rates (per 1 USD):");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("   ------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"     $   CNY: {exchangeRates["CNY"]:N2}");
            Console.WriteLine($"     $   BDT: {exchangeRates["BDT"]:N2}");
            Console.WriteLine($"     $   SAR: {exchangeRates["SAR"]:N2}");
            Console.WriteLine($"     $   EUR: {exchangeRates["EUR"]:N2}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  ## Contact:");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("   ------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("    Customer Service : 3-258-MD ABDULLAH");
            Console.WriteLine("    Email            : support@abdullahbank.com");
            Console.WriteLine("    Website URL      : www.abdullahbank.com/student");
            Console.WriteLine("    WeChat           : AbdullahBank_Support");
            Console.WriteLine("    Phone            : +86 1772 164 6613");
            Console.WriteLine("    Int Helpline     : +86 5138 501 2794");
            Console.WriteLine("    Address          : 9 Seyuan Road, Chongchuan District,Nantong");
            Console.ResetColor();

            Pause();
        }

        static void Pause()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("\n   Press any key...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
