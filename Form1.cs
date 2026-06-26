using System;
using CyberSecurityAwarenessBot.Models;
using CyberSecurityAwarenessBot.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using Font = System.Drawing.Font;

namespace CyberSecurityAwarenessBot
{
    public partial class Form1 : Form
    {
        // ── Colours ────────────────────────────────────────────────
        private readonly Color BgLight = Color.FromArgb(240, 240, 240);
        private readonly Color BgWhite = Color.White;
        private readonly Color BgHeader = Color.FromArgb(30, 30, 30);
        private readonly Color AccentGreen = Color.FromArgb(0, 150, 80);
        private readonly Color AccentBlue = Color.FromArgb(0, 120, 215);
        private readonly Color TextDark = Color.FromArgb(20, 20, 20);
        private readonly Color TextGray = Color.FromArgb(100, 100, 100);
        private readonly Color BorderGray = Color.FromArgb(200, 200, 200);
        private readonly Color BotMsgBg = Color.FromArgb(230, 245, 255);
        private readonly Color UserMsgBg = Color.FromArgb(220, 255, 220);
        private readonly Color TabActiveBg = Color.FromArgb(0, 150, 80);
        private readonly Color TabInactiveBg = Color.FromArgb(60, 60, 60);

        // ── Services ───────────────────────────────────────────────
        private readonly UserProfile _user = new UserProfile();
        private readonly ChatbotService _chatbot = new ChatbotService();
        private readonly DatabaseService _db = new DatabaseService();
        private readonly NlpService _nlp = new NlpService();
        private readonly QuizService _quiz = new QuizService();

        // ── State ──────────────────────────────────────────────────
        private bool _nameSet = false;
        private bool _awaitingReminder = false;
        private string _pendingTaskTitle = "";
        private string _pendingTaskDesc = "";
        private string _currentTab = "Tasks";

        private readonly string PlaceholderInput = "start quiz, show activity log...";
        private readonly string PlaceholderTitle = "Task title";
        private readonly string PlaceholderDesc = "Describe the cybersecurity task...";

        // ── Controls ───────────────────────────────────────────────
        private Panel pnlNameScreen = null;
        private Panel pnlMain = null;
        private Panel pnlHeader = null;
        private Panel pnlTabs = null;
        private Panel pnlContent = null;
        private Panel pnlChat = null;
        private Panel pnlRight = null;
        private FlowLayoutPanel flpMessages = null;
        private TextBox txtInput = null;
        private Button btnSend = null;

        // Name screen
        private TextBox txtName = null;
        private Button btnConnect = null;
        private Label lblNameErr = null;

        // Task panel
        private TextBox txtTaskTitle = null;
        private TextBox txtTaskDesc = null;
        private CheckBox chkReminder = null;
        private DateTimePicker dtpReminder = null;
        private Button btnAddTask = null;
        private ListView lvTasks = null;
        private Button btnMarkDone = null;
        private Button btnDeleteTask = null;
        private Label lblStorage = null;

        // Quiz panel
        private Panel pnlQuiz = null;
        private Label lblQuizQuestion = null;
        private Button btnOptA = null;
        private Button btnOptB = null;
        private Button btnOptC = null;
        private Button btnOptD = null;
        private Label lblQuizFeedback = null;
        private Label lblQuizScore = null;

        // Log panel
        private Panel pnlLog = null;
        private ListView lvLog = null;

        // Tab buttons
        private Button btnTabTasks = null;
        private Button btnTabQuiz = null;
        private Button btnTabLog = null;

        // Status bar
        private Label lblFavTopic = null;
        private Label lblCurrentTopic = null;

        // Audio
        private SoundPlayer _soundPlayer = null;

        private System.Windows.Forms.Timer _clockTimer = null;
        private object reader;

        // ══════════════════════════════════════════════════════════
        public Form1()
        {
            InitializeComponent();
            BuildNameScreen();
            BuildMainScreen();
            ShowNameScreen();

            _clockTimer = new System.Windows.Forms.Timer();
            _clockTimer.Interval = 1000;
            _clockTimer.Tick += new EventHandler(ClockTimer_Tick);
            _clockTimer.Start();
        }

        private void ClockTimer_Tick(object sender, EventArgs e) { }

        // ══════════════════════════════════════════════════════════
        //  AUDIO
        // ══════════════════════════════════════════════════════════
        private void PlayGreeting()
        {
            try
            {
                string path1 = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Assets", "Greetings.wav");
                string path2 = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory, "Assets", "Greetings.wav");
                string path3 = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Greetings.wav");

                string foundPath = null;
                if (File.Exists(path1)) foundPath = path1;
                else if (File.Exists(path2)) foundPath = path2;
                else if (File.Exists(path3)) foundPath = path3;

                if (foundPath != null)
                {
                    if (_soundPlayer != null)
                    {
                        _soundPlayer.Stop();
                        _soundPlayer.Dispose();
                    }
                    _soundPlayer = new SoundPlayer(foundPath);
                    _soundPlayer.Load();
                    _soundPlayer.Play();
                }
                else
                {
                    MessageBox.Show(
                        "Greetings.wav not found.\n\nLooked in:\n" +
                        path1 + "\n" + path2 + "\n" + path3,
                        "Audio",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Audio error: " + ex.Message,
                    "Audio Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ══════════════════════════════════════════════════════════
        //  NAME SCREEN
        // ══════════════════════════════════════════════════════════
        private void BuildNameScreen()
        {
            pnlNameScreen = new Panel();
            pnlNameScreen.Dock = DockStyle.Fill;
            pnlNameScreen.BackColor = BgLight;

            TableLayoutPanel tbl = new TableLayoutPanel();
            tbl.Dock = DockStyle.Fill;
            tbl.ColumnCount = 1;
            tbl.RowCount = 6;
            tbl.BackColor = Color.Transparent;
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 25));

            Label lblTitle = new Label();
            lblTitle.Text = "CYBERSECURITY AWARENESS BOT";
            lblTitle.Font = new System.Drawing.Font("Consolas", 16f, FontStyle.Bold);
            lblTitle.ForeColor = BgHeader;
            lblTitle.AutoSize = false;
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            Label lblSub = new Label();
            lblSub.Text = "Enter your name to begin:";
            lblSub.Font = new Font("Segoe UI", 10f);
            lblSub.ForeColor = TextGray;
            lblSub.AutoSize = false;
            lblSub.Dock = DockStyle.Fill;
            lblSub.TextAlign = ContentAlignment.MiddleCenter;
            lblSub.Height = 30;

            txtName = new TextBox();
            txtName.Font = new Font("Segoe UI", 12f);
            txtName.BorderStyle = BorderStyle.FixedSingle;
            txtName.TextAlign = HorizontalAlignment.Center;
            txtName.MaxLength = 40;
            txtName.Anchor = AnchorStyles.None;
            txtName.Width = 280;
            txtName.KeyDown += new KeyEventHandler(TxtName_KeyDown);

            lblNameErr = new Label();
            lblNameErr.Text = "";
            lblNameErr.Font = new Font("Segoe UI", 9f);
            lblNameErr.ForeColor = Color.Red;
            lblNameErr.AutoSize = false;
            lblNameErr.Height = 22;
            lblNameErr.Dock = DockStyle.Fill;
            lblNameErr.TextAlign = ContentAlignment.TopCenter;

            btnConnect = new Button();
            btnConnect.Text = "START";
            btnConnect.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            btnConnect.ForeColor = Color.White;
            btnConnect.BackColor = AccentGreen;
            btnConnect.FlatStyle = FlatStyle.Flat;
            btnConnect.FlatAppearance.BorderSize = 0;
            btnConnect.Anchor = AnchorStyles.None;
            btnConnect.Size = new Size(160, 40);
            btnConnect.Cursor = Cursors.Hand;
            btnConnect.Click += new EventHandler(BtnConnect_Click);

            tbl.Controls.Add(
                new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent }, 0, 0);
            tbl.Controls.Add(lblTitle, 0, 1);
            tbl.Controls.Add(lblSub, 0, 2);
            tbl.Controls.Add(WrapCenter(txtName, 280, 36), 0, 3);
            tbl.Controls.Add(lblNameErr, 0, 4);
            tbl.Controls.Add(WrapCenter(btnConnect, 160, 44), 0, 5);

            pnlNameScreen.Controls.Add(tbl);
            Controls.Add(pnlNameScreen);
        }

        // ══════════════════════════════════════════════════════════
        //  MAIN SCREEN
        // ══════════════════════════════════════════════════════════
        private void BuildMainScreen()
        {
            Text = "Cybersecurity Awareness Bot";
            Size = new Size(1050, 700);
            MinimumSize = new Size(900, 600);
            BackColor = BgLight;
            StartPosition = FormStartPosition.CenterScreen;

            pnlMain = new Panel();
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Visible = false;

            // ── Header ────────────────────────────────────────────
            pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 90;
            pnlHeader.BackColor = BgHeader;
            pnlHeader.Padding = new Padding(10, 5, 10, 5);

            Panel pnlHeaderInner = new Panel();
            pnlHeaderInner.Dock = DockStyle.Fill;
            pnlHeaderInner.BackColor = Color.Transparent;

            Label lblAscii = new Label();
            lblAscii.Text =
                "************************************************************\r\n" +
                "         CYBERSECURITY AWARENESS BOT\r\n" +
                "  Tasks  |  Reminders  |  Quiz  |  Activity Log\r\n" +
                "************************************************************\r\n" +
                "        Stay alert. Stay secure. Stay informed.";
            lblAscii.Font = new Font("Consolas", 7.5f);
            lblAscii.ForeColor = Color.LightGreen;
            lblAscii.AutoSize = false;
            lblAscii.Dock = DockStyle.Fill;
            lblAscii.TextAlign = ContentAlignment.MiddleCenter;

            // Audio button in header
            Button btnHeaderAudio = new Button();
            btnHeaderAudio.Text = "♪ Play Greeting";
            btnHeaderAudio.Font = new Font("Segoe UI", 8f);
            btnHeaderAudio.ForeColor = Color.White;
            btnHeaderAudio.BackColor = Color.FromArgb(0, 100, 60);
            btnHeaderAudio.FlatStyle = FlatStyle.Flat;
            btnHeaderAudio.FlatAppearance.BorderSize = 0;
            btnHeaderAudio.Dock = DockStyle.Right;
            btnHeaderAudio.Width = 110;
            btnHeaderAudio.Cursor = Cursors.Hand;
            btnHeaderAudio.Click += new EventHandler(BtnAudio_Click);

            pnlHeaderInner.Controls.Add(lblAscii);
            pnlHeaderInner.Controls.Add(btnHeaderAudio);
            pnlHeader.Controls.Add(pnlHeaderInner);

            // ── Status bar ────────────────────────────────────────
            Panel pnlStatus = new Panel();
            pnlStatus.Dock = DockStyle.Top;
            pnlStatus.Height = 26;
            pnlStatus.BackColor = Color.FromArgb(245, 245, 245);
            pnlStatus.Padding = new Padding(8, 4, 8, 0);

            lblFavTopic = new Label();
            lblFavTopic.Text = "favourite topic yet";
            lblFavTopic.Font = new System.Drawing.Font("Segoe UI", 8.5f);
            lblFavTopic.ForeColor = TextGray;
            lblFavTopic.AutoSize = true;
            lblFavTopic.Dock = DockStyle.Left;

            Label lblSep = new Label();
            lblSep.Text = " | ";
            lblSep.Font = new Font("Segoe UI", 8.5f);
            lblSep.ForeColor = TextGray;
            lblSep.AutoSize = true;
            lblSep.Dock = DockStyle.Left;

            lblCurrentTopic = new Label();
            lblCurrentTopic.Text = "current topic = no recent topic";
            lblCurrentTopic.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblCurrentTopic.ForeColor = TextDark;
            lblCurrentTopic.AutoSize = true;
            lblCurrentTopic.Dock = DockStyle.Left;

            pnlStatus.Controls.Add(lblCurrentTopic);
            pnlStatus.Controls.Add(lblSep);
            pnlStatus.Controls.Add(lblFavTopic);

            // ── Tab bar ───────────────────────────────────────────
            pnlTabs = new Panel();
            pnlTabs.Dock = DockStyle.Top;
            pnlTabs.Height = 34;
            pnlTabs.BackColor = Color.FromArgb(50, 50, 50);

            btnTabTasks = MakeTabButton("Task Assistant");
            btnTabTasks.Click += new EventHandler(BtnTabTasks_Click);

            btnTabQuiz = MakeTabButton("Mini Game");
            btnTabQuiz.Click += new EventHandler(BtnTabQuiz_Click);

            btnTabLog = MakeTabButton("Activity Log");
            btnTabLog.Click += new EventHandler(BtnTabLog_Click);

            btnTabTasks.Left = 0; btnTabTasks.Top = 0;
            btnTabQuiz.Left = 122; btnTabQuiz.Top = 0;
            btnTabLog.Left = 244; btnTabLog.Top = 0;

            pnlTabs.Controls.Add(btnTabTasks);
            pnlTabs.Controls.Add(btnTabQuiz);
            pnlTabs.Controls.Add(btnTabLog);

            // ── Content area ──────────────────────────────────────
            pnlContent = new Panel();
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.BackColor = BgLight;

            // ── Chat (left) ───────────────────────────────────────
            pnlChat = new Panel();
            pnlChat.Dock = DockStyle.Fill;
            pnlChat.BackColor = BgWhite;

            flpMessages = new FlowLayoutPanel();
            flpMessages.Dock = DockStyle.Fill;
            flpMessages.FlowDirection = FlowDirection.TopDown;
            flpMessages.WrapContents = false;
            flpMessages.AutoScroll = true;
            flpMessages.Padding = new Padding(8);
            flpMessages.BackColor = BgWhite;

            Panel pnlInputRow = new Panel();
            pnlInputRow.Dock = DockStyle.Bottom;
            pnlInputRow.Height = 46;
            pnlInputRow.BackColor = Color.FromArgb(240, 240, 240);
            pnlInputRow.Padding = new Padding(8, 6, 8, 6);

            txtInput = new TextBox();
            txtInput.Dock = DockStyle.Fill;
            txtInput.Font = new Font("Segoe UI", 10f);
            txtInput.BorderStyle = BorderStyle.FixedSingle;
            txtInput.ForeColor = TextGray;
            txtInput.Text = PlaceholderInput;
            txtInput.GotFocus += new EventHandler(TxtInput_GotFocus);
            txtInput.LostFocus += new EventHandler(TxtInput_LostFocus);
            txtInput.KeyDown += new KeyEventHandler(TxtInput_KeyDown);

            btnSend = new Button();
            btnSend.Text = "Send";
            btnSend.Dock = DockStyle.Right;
            btnSend.Width = 70;
            btnSend.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
            btnSend.BackColor = AccentBlue;
            btnSend.ForeColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Cursor = Cursors.Hand;
            btnSend.Click += new EventHandler(BtnSend_Click);

            pnlInputRow.Controls.Add(txtInput);
            pnlInputRow.Controls.Add(btnSend);

            pnlChat.Controls.Add(flpMessages);
            pnlChat.Controls.Add(pnlInputRow);

            // ── Right panel ───────────────────────────────────────
            BuildRightPanel();

            pnlContent.Controls.Add(pnlChat);
            pnlContent.Controls.Add(pnlRight);

            pnlMain.Controls.Add(pnlContent);
            pnlMain.Controls.Add(pnlTabs);
            pnlMain.Controls.Add(pnlStatus);
            pnlMain.Controls.Add(pnlHeader);

            Controls.Add(pnlMain);
            SetActiveTab("Tasks");
        }

        // ══════════════════════════════════════════════════════════
        //  RIGHT PANEL
        // ══════════════════════════════════════════════════════════
        private void BuildRightPanel()
        {
            pnlRight = new Panel();
            pnlRight.Dock = DockStyle.Right;
            pnlRight.Width = 310;
            pnlRight.BackColor = Color.FromArgb(248, 248, 248);
            pnlRight.Padding = new Padding(10);
            pnlRight.BorderStyle = BorderStyle.FixedSingle;

            BuildTaskControls();
            BuildQuizPanel();
            BuildLogPanel();
        }

        private void BuildTaskControls()
        {
            Label lblPanelTitle = new Label();
            lblPanelTitle.Text = "Task Assistant";
            lblPanelTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblPanelTitle.ForeColor = TextDark;
            lblPanelTitle.Dock = DockStyle.Top;
            lblPanelTitle.Height = 28;

            Label lblTitleField = new Label();
            lblTitleField.Text = "Task title";
            lblTitleField.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblTitleField.ForeColor = TextGray;
            lblTitleField.Dock = DockStyle.Top;
            lblTitleField.Height = 18;

            txtTaskTitle = new TextBox();
            txtTaskTitle.Font = new Font("Segoe UI", 9.5f);
            txtTaskTitle.BorderStyle = BorderStyle.FixedSingle;
            txtTaskTitle.Dock = DockStyle.Top;
            txtTaskTitle.Height = 26;
            txtTaskTitle.ForeColor = TextGray;
            txtTaskTitle.Text = PlaceholderTitle;
            txtTaskTitle.GotFocus += new EventHandler(TxtTaskTitle_GotFocus);
            txtTaskTitle.LostFocus += new EventHandler(TxtTaskTitle_LostFocus);

            Label lblDescField = new Label();
            lblDescField.Text = "Description";
            lblDescField.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblDescField.ForeColor = TextGray;
            lblDescField.Dock = DockStyle.Top;
            lblDescField.Height = 18;

            txtTaskDesc = new TextBox();
            txtTaskDesc.Font = new Font("Segoe UI", 9f);
            txtTaskDesc.BorderStyle = BorderStyle.FixedSingle;
            txtTaskDesc.Dock = DockStyle.Top;
            txtTaskDesc.Multiline = true;
            txtTaskDesc.Height = 60;
            txtTaskDesc.ForeColor = TextGray;
            txtTaskDesc.Text = PlaceholderDesc;
            txtTaskDesc.GotFocus += new EventHandler(TxtTaskDesc_GotFocus);
            txtTaskDesc.LostFocus += new EventHandler(TxtTaskDesc_LostFocus);

            Panel pnlRem = new Panel();
            pnlRem.Dock = DockStyle.Top;
            pnlRem.Height = 28;
            pnlRem.BackColor = Color.Transparent;

            chkReminder = new CheckBox();
            chkReminder.Text = "Set reminder";
            chkReminder.Font = new Font("Segoe UI", 9f);
            chkReminder.ForeColor = TextDark;
            chkReminder.Dock = DockStyle.Left;
            chkReminder.Width = 105;
            chkReminder.CheckedChanged += new EventHandler(ChkReminder_Changed);

            dtpReminder = new DateTimePicker();
            dtpReminder.Dock = DockStyle.Fill;
            dtpReminder.Font = new Font("Segoe UI", 8.5f);
            dtpReminder.Format = DateTimePickerFormat.Short;
            dtpReminder.Visible = false;

            pnlRem.Controls.Add(dtpReminder);
            pnlRem.Controls.Add(chkReminder);

            btnAddTask = new Button();
            btnAddTask.Text = "Add Task";
            btnAddTask.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnAddTask.ForeColor = Color.White;
            btnAddTask.BackColor = AccentGreen;
            btnAddTask.FlatStyle = FlatStyle.Flat;
            btnAddTask.FlatAppearance.BorderSize = 0;
            btnAddTask.Dock = DockStyle.Top;
            btnAddTask.Height = 34;
            btnAddTask.Cursor = Cursors.Hand;
            btnAddTask.Click += new EventHandler(BtnAddTask_Click);

            Label lblTaskList = new Label();
            lblTaskList.Text = "Title";
            lblTaskList.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            lblTaskList.ForeColor = TextGray;
            lblTaskList.Dock = DockStyle.Top;
            lblTaskList.Height = 18;

            lvTasks = new ListView();
            lvTasks.Dock = DockStyle.Top;
            lvTasks.Height = 130;
            lvTasks.View = View.Details;
            lvTasks.FullRowSelect = true;
            lvTasks.GridLines = true;
            lvTasks.Font = new Font("Segoe UI", 8.5f);
            lvTasks.BorderStyle = BorderStyle.FixedSingle;
            lvTasks.Columns.Add("ID", 28);
            lvTasks.Columns.Add("Title", 130);
            lvTasks.Columns.Add("Done", 44);
            lvTasks.Columns.Add("Rem.", 66);

            Panel pnlActions = new Panel();
            pnlActions.Dock = DockStyle.Top;
            pnlActions.Height = 34;
            pnlActions.BackColor = Color.Transparent;

            btnMarkDone = new Button();
            btnMarkDone.Text = "Mark";
            btnMarkDone.Font = new Font("Segoe UI", 9f);
            btnMarkDone.BackColor = AccentGreen;
            btnMarkDone.ForeColor = Color.White;
            btnMarkDone.FlatStyle = FlatStyle.Flat;
            btnMarkDone.FlatAppearance.BorderSize = 0;
            btnMarkDone.Width = 70;
            btnMarkDone.Height = 30;
            btnMarkDone.Left = 0;
            btnMarkDone.Cursor = Cursors.Hand;
            btnMarkDone.Click += new EventHandler(BtnMarkDone_Click);

            btnDeleteTask = new Button();
            btnDeleteTask.Text = "Delete";
            btnDeleteTask.Font = new Font("Segoe UI", 9f);
            btnDeleteTask.BackColor = Color.FromArgb(200, 50, 50);
            btnDeleteTask.ForeColor = Color.White;
            btnDeleteTask.FlatStyle = FlatStyle.Flat;
            btnDeleteTask.FlatAppearance.BorderSize = 0;
            btnDeleteTask.Width = 70;
            btnDeleteTask.Height = 30;
            btnDeleteTask.Left = 74;
            btnDeleteTask.Cursor = Cursors.Hand;
            btnDeleteTask.Click += new EventHandler(BtnDeleteTask_Click);

            pnlActions.Controls.Add(btnMarkDone);
            pnlActions.Controls.Add(btnDeleteTask);

            lblStorage = new Label();
            lblStorage.Text = "Storage: Local fallback, no DB";
            lblStorage.Font = new Font("Segoe UI", 7.5f);
            lblStorage.ForeColor = TextGray;
            lblStorage.Dock = DockStyle.Top;
            lblStorage.Height = 18;

            // Add controls top-to-bottom (DockStyle.Top stacks in reverse)
            pnlRight.Controls.Add(lblStorage);
            pnlRight.Controls.Add(pnlActions);
            pnlRight.Controls.Add(lvTasks);
            pnlRight.Controls.Add(lblTaskList);
            pnlRight.Controls.Add(btnAddTask);
            pnlRight.Controls.Add(pnlRem);
            pnlRight.Controls.Add(txtTaskDesc);
            pnlRight.Controls.Add(lblDescField);
            pnlRight.Controls.Add(txtTaskTitle);
            pnlRight.Controls.Add(lblTitleField);
            pnlRight.Controls.Add(lblPanelTitle);
        }

        private void BuildQuizPanel()
        {
            pnlQuiz = new Panel();
            pnlQuiz.Dock = DockStyle.Fill;
            pnlQuiz.BackColor = Color.FromArgb(248, 248, 248);
            pnlQuiz.Padding = new Padding(10);
            pnlQuiz.Visible = false;

            Label lblQt = new Label();
            lblQt.Text = "Cybersecurity Quiz";
            lblQt.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblQt.ForeColor = TextDark;
            lblQt.Dock = DockStyle.Top;
            lblQt.Height = 28;

            lblQuizScore = new Label();
            lblQuizScore.Text = "Score: 0 / 0";
            lblQuizScore.Font = new Font("Segoe UI", 9f);
            lblQuizScore.ForeColor = TextGray;
            lblQuizScore.Dock = DockStyle.Top;
            lblQuizScore.Height = 22;

            lblQuizQuestion = new Label();
            lblQuizQuestion.Text = "Type 'start quiz' in chat to begin.";
            lblQuizQuestion.Font = new Font("Segoe UI", 9.5f);
            lblQuizQuestion.ForeColor = TextDark;
            lblQuizQuestion.Dock = DockStyle.Top;
            lblQuizQuestion.Height = 80;
            lblQuizQuestion.AutoSize = false;

            btnOptA = MakeQuizOption("A");
            btnOptB = MakeQuizOption("B");
            btnOptC = MakeQuizOption("C");
            btnOptD = MakeQuizOption("D");

            btnOptA.Click += new EventHandler(BtnOpt_Click);
            btnOptB.Click += new EventHandler(BtnOpt_Click);
            btnOptC.Click += new EventHandler(BtnOpt_Click);
            btnOptD.Click += new EventHandler(BtnOpt_Click);

            lblQuizFeedback = new Label();
            lblQuizFeedback.Text = "";
            lblQuizFeedback.Font = new Font("Segoe UI", 9f, FontStyle.Italic);
            lblQuizFeedback.ForeColor = AccentGreen;
            lblQuizFeedback.Dock = DockStyle.Top;
            lblQuizFeedback.Height = 60;
            lblQuizFeedback.AutoSize = false;

            pnlQuiz.Controls.Add(lblQuizFeedback);
            pnlQuiz.Controls.Add(btnOptD);
            pnlQuiz.Controls.Add(btnOptC);
            pnlQuiz.Controls.Add(btnOptB);
            pnlQuiz.Controls.Add(btnOptA);
            pnlQuiz.Controls.Add(lblQuizQuestion);
            pnlQuiz.Controls.Add(lblQuizScore);
            pnlQuiz.Controls.Add(lblQt);

            pnlRight.Controls.Add(pnlQuiz);
        }

        private void BuildLogPanel()
        {
            pnlLog = new Panel();
            pnlLog.Dock = DockStyle.Fill;
            pnlLog.BackColor = Color.FromArgb(248, 248, 248);
            pnlLog.Padding = new Padding(10);
            pnlLog.Visible = false;

            Label lblLogTitle = new Label();
            lblLogTitle.Text = "Activity Log";
            lblLogTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
            lblLogTitle.ForeColor = TextDark;
            lblLogTitle.Dock = DockStyle.Top;
            lblLogTitle.Height = 28;

            lvLog = new ListView();
            lvLog.Dock = DockStyle.Fill;
            lvLog.View = View.Details;
            lvLog.FullRowSelect = true;
            lvLog.GridLines = true;
            lvLog.Font = new Font("Segoe UI", 8.5f);
            lvLog.BorderStyle = BorderStyle.FixedSingle;
            lvLog.Columns.Add("Time", 70);
            lvLog.Columns.Add("Action", 200);

            pnlLog.Controls.Add(lvLog);
            pnlLog.Controls.Add(lblLogTitle);

            pnlRight.Controls.Add(pnlLog);
        }

        // ══════════════════════════════════════════════════════════
        //  TAB SWITCHING
        // ══════════════════════════════════════════════════════════
        private void SetActiveTab(string tab)
        {
            _currentTab = tab;

            btnTabTasks.BackColor = TabInactiveBg;
            btnTabQuiz.BackColor = TabInactiveBg;
            btnTabLog.BackColor = TabInactiveBg;

            pnlQuiz.Visible = false;
            pnlLog.Visible = false;

            bool showTaskControls = (tab == "Tasks");
            foreach (Control c in pnlRight.Controls)
            {
                if (c == pnlQuiz || c == pnlLog) continue;
                c.Visible = showTaskControls;
            }

            switch (tab)
            {
                case "Tasks":
                    btnTabTasks.BackColor = TabActiveBg;
                    break;
                case "Quiz":
                    pnlQuiz.Visible = true;
                    btnTabQuiz.BackColor = TabActiveBg;
                    break;
                case "Log":
                    pnlLog.Visible = true;
                    btnTabLog.BackColor = TabActiveBg;
                    RefreshLogView();
                    break;
            }
        }

        private void BtnTabTasks_Click(object sender, EventArgs e) { SetActiveTab("Tasks"); }
        private void BtnTabQuiz_Click(object sender, EventArgs e) { SetActiveTab("Quiz"); }
        private void BtnTabLog_Click(object sender, EventArgs e) { SetActiveTab("Log"); }

        // ══════════════════════════════════════════════════════════
        //  SCREEN SWITCHING
        // ══════════════════════════════════════════════════════════
        private void ShowNameScreen()
        {
            pnlNameScreen.Visible = true;
            if (pnlMain != null) pnlMain.Visible = false;
            txtName.Focus();
        }

        private void ShowMainScreen()
        {
            pnlNameScreen.Visible = false;
            pnlMain.Visible = true;

            // Play greeting as soon as main screen appears
            PlayGreeting();

            AddBotBubble(
                "Hello, " + _user.Name + "! I am your Cybersecurity Awareness Bot.");
            AddBotBubble(
                "I can help with tasks, reminders, a quiz mini-game, and an activity log. " +
                "What is your name? (already set to " + _user.Name + ")");
            AddBotBubble(
                "For tips, say 'add task to enable 2FA', " +
                "'remind me to update my password'");

            _user.LogAction("Session started");
            RefreshTaskList();
            txtInput.Focus();
        }

        // ══════════════════════════════════════════════════════════
        //  TASK PANEL EVENTS
        // ══════════════════════════════════════════════════════════
        private void BtnAddTask_Click(object sender, EventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            string desc = txtTaskDesc.Text.Trim();

            if (string.IsNullOrWhiteSpace(title) || title == PlaceholderTitle)
            {
                AddBotBubble("Please enter a task title.");
                return;
            }
            if (string.IsNullOrWhiteSpace(desc) || desc == PlaceholderDesc)
                desc = BuildDescription(title);

            TaskItem task = new TaskItem();
            task.Title = title;
            task.Description = desc;

            if (chkReminder.Checked)
            {
                task.ReminderDate = dtpReminder.Value;
                task.Reminder = dtpReminder.Value.ToString("dd MMM yyyy");
            }

            int id = _db.AddTask(task);
            _user.LogAction("Task added: '" + title + "'");

            AddBotBubble(id > 0
                ? "Task added with the description \"" + desc +
                  "\" Would you like a reminder?"
                : "Task noted locally: \"" + desc + "\"");

            txtTaskTitle.Text = PlaceholderTitle;
            txtTaskTitle.ForeColor = TextGray;
            txtTaskDesc.Text = PlaceholderDesc;
            txtTaskDesc.ForeColor = TextGray;
            chkReminder.Checked = false;

            RefreshTaskList();
            lblStorage.Text = id > 0
                ? "Storage: MySQL (ID #" + id + ")"
                : "Storage: Local fallback, no DB";
        }

        private void BtnMarkDone_Click(object sender, EventArgs e)
        {
            if (lvTasks.SelectedItems.Count == 0)
            {
                AddBotBubble("Please select a task from the list first.");
                return;
            }
            int id = int.Parse(lvTasks.SelectedItems[0].Text);
            _db.MarkCompleted(id);
            _user.LogAction("Task #" + id + " marked complete");
            AddBotBubble("Task #" + id + " marked as completed.");
            RefreshTaskList();
        }

        private void BtnDeleteTask_Click(object sender, EventArgs e)
        {
            if (lvTasks.SelectedItems.Count == 0)
            {
                AddBotBubble("Please select a task from the list first.");
                return;
            }
            int id = int.Parse(lvTasks.SelectedItems[0].Text);
            _db.DeleteTask(id);
            _user.LogAction("Task #" + id + " deleted");
            AddBotBubble("Task #" + id + " deleted.");
            RefreshTaskList();
        }

        private void ChkReminder_Changed(object sender, EventArgs e)
        {
            dtpReminder.Visible = chkReminder.Checked;
        }

        private void RefreshTaskList()
        {
            lvTasks.Items.Clear();
            List<TaskItem> tasks = _db.GetAllTasks(reader);
            foreach (TaskItem t in tasks)
            {
                ListViewItem item = new ListViewItem(t.Id.ToString());
                item.SubItems.Add(t.Title);
                item.SubItems.Add(t.IsCompleted ? "Yes" : "No");
                item.SubItems.Add(t.Reminder != null ? t.Reminder : "-");
                if (t.IsCompleted) item.ForeColor = TextGray;
                lvTasks.Items.Add(item);
            }
            lblStorage.Text = tasks.Count > 0
                ? "Storage: MySQL (" + tasks.Count + " task(s))"
                : "Storage: Local fallback, no DB";
        }

        // ══════════════════════════════════════════════════════════
        //  QUIZ PANEL EVENTS
        // ══════════════════════════════════════════════════════════
        private void BtnOpt_Click(object sender, EventArgs e)
        {
            if (!_quiz.IsActive || _quiz.IsFinished) return;

            Button b = (Button)sender;
            QuizQuestion q = _quiz.CurrentQuestion();
            if (q == null) return;

            int choiceIndex = -1;
            if (q.IsTrueFalse)
            {
                if (b == btnOptA) choiceIndex = 0;
                else if (b == btnOptB) choiceIndex = 1;
            }
            else
            {
                if (b == btnOptA) choiceIndex = 0;
                else if (b == btnOptB) choiceIndex = 1;
                else if (b == btnOptC) choiceIndex = 2;
                else if (b == btnOptD) choiceIndex = 3;
            }

            if (choiceIndex == -1) return;

            KeyValuePair<bool, string> result = _quiz.Answer(choiceIndex);
            bool correct = result.Key;
            string explanation = result.Value;

            lblQuizFeedback.Text = correct
                ? "Correct! " + explanation
                : "Incorrect. " + explanation;
            lblQuizFeedback.ForeColor = correct ? AccentGreen : Color.Red;

            AddBotBubble(correct
                ? "Correct! " + explanation
                : "Incorrect. " + explanation);

            lblQuizScore.Text = "Score: " + _quiz.Score + " / " + _quiz.TotalQuestions;

            if (_quiz.IsFinished)
            {
                _quiz.Stop();
                _user.LogAction("Quiz completed - Score: " +
                    _quiz.Score + "/" + _quiz.TotalQuestions);
                string feedback = _quiz.FinalFeedback();
                AddBotBubble(feedback);
                lblQuizQuestion.Text = feedback;
                SetQuizButtonsEnabled(false);
            }
            else
            {
                ShowQuizQuestion();
            }
        }

        private void ShowQuizQuestion()
        {
            QuizQuestion q = _quiz.CurrentQuestion();
            if (q == null) return;

            int qNum = _quiz.CurrentIndex + 1;
            lblQuizQuestion.Text = "Q" + qNum + "/" + _quiz.TotalQuestions +
                                   ": " + q.Question;
            if (q.IsTrueFalse)
            {
                btnOptA.Text = "True";
                btnOptB.Text = "False";
                btnOptC.Visible = false;
                btnOptD.Visible = false;
            }
            else
            {
                btnOptA.Text = q.Options.Length > 0 ? q.Options[0] : "A";
                btnOptB.Text = q.Options.Length > 1 ? q.Options[1] : "B";
                btnOptC.Text = q.Options.Length > 2 ? q.Options[2] : "C";
                btnOptD.Text = q.Options.Length > 3 ? q.Options[3] : "D";
                btnOptC.Visible = true;
                btnOptD.Visible = true;
            }
            lblQuizFeedback.Text = "";
            SetQuizButtonsEnabled(true);
        }

        private void SetQuizButtonsEnabled(bool enabled)
        {
            btnOptA.Enabled = enabled;
            btnOptB.Enabled = enabled;
            btnOptC.Enabled = enabled;
            btnOptD.Enabled = enabled;
        }

        // ══════════════════════════════════════════════════════════
        //  ACTIVITY LOG
        // ══════════════════════════════════════════════════════════
        private void RefreshLogView()
        {
            lvLog.Items.Clear();
            List<ActivityLogEntry> log = _user.GetRecentLog(10);
            foreach (ActivityLogEntry entry in log)
            {
                ListViewItem item = new ListViewItem(
                    entry.Timestamp.ToString("HH:mm:ss"));
                item.SubItems.Add(entry.Description);
                lvLog.Items.Add(item);
            }
        }

        // ══════════════════════════════════════════════════════════
        //  MAIN CHAT DISPATCHER
        // ══════════════════════════════════════════════════════════
        private void SendMessage()
        {
            string text = txtInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(text) || text == PlaceholderInput) return;

            txtInput.Text = "";
            txtInput.ForeColor = TextGray;

            AddUserBubble(text);
            _user.AddToHistory(text);
            lblCurrentTopic.Text =
                "current topic = " + text.Substring(0, Math.Min(text.Length, 30));

            if (_awaitingReminder)
            {
                HandleReminderResponse(text);
                return;
            }

            UserIntent intent = _nlp.Detect(text);

            switch (intent)
            {
                case UserIntent.AddTask:
                    HandleAddTask(text);
                    SetActiveTab("Tasks");
                    break;
                case UserIntent.ViewTasks:
                    HandleViewTasks();
                    SetActiveTab("Tasks");
                    break;
                case UserIntent.DeleteTask:
                    HandleDeleteTask(text);
                    break;
                case UserIntent.CompleteTask:
                    HandleCompleteTask(text);
                    break;
                case UserIntent.SetReminder:
                    HandleSetReminder(text);
                    break;
                case UserIntent.StartQuiz:
                    HandleStartQuiz();
                    SetActiveTab("Quiz");
                    break;
                case UserIntent.ShowActivityLog:
                    HandleShowActivityLog();
                    SetActiveTab("Log");
                    break;
                case UserIntent.ShowHistory:
                    ShowHistory();
                    break;
                case UserIntent.Help:
                    AddBotBubble(
                        "Commands:\n" +
                        "  add task [title]     - add a cybersecurity task\n" +
                        "  show tasks           - list all tasks\n" +
                        "  complete task #1     - mark a task done\n" +
                        "  delete task #1       - remove a task\n" +
                        "  start quiz           - begin the quiz\n" +
                        "  show activity log    - view recent actions\n" +
                        "  history              - view question history\n\n" +
                        "Or ask about: passwords, phishing, safe browsing,\n" +
                        "              ransomware, data protection, 2FA");
                    break;
                case UserIntent.Exit:
                    AddBotBubble("Goodbye, " + _user.Name + "! Stay safe online.");
                    _user.LogAction("Session ended");
                    break;
                default:
                    string reply = _chatbot.GetBotResponse(text);
                    _user.LogAction("Topic: '" +
                        text.Substring(0, Math.Min(text.Length, 30)) + "'");
                    AddBotBubble(reply);
                    break;
            }
        }

        // ══════════════════════════════════════════════════════════
        //  TASK CHAT HANDLERS
        // ══════════════════════════════════════════════════════════
        private void HandleAddTask(string input)
        {
            string title = _nlp.ExtractTaskTitle(input);
            if (string.IsNullOrWhiteSpace(title) || title.Length < 3)
            {
                AddBotBubble("What task? e.g. 'add task - Enable 2FA'");
                return;
            }
            _pendingTaskTitle = title;
            _pendingTaskDesc = BuildDescription(title);
            _awaitingReminder = true;

            txtTaskTitle.Text = title;
            txtTaskTitle.ForeColor = TextDark;
            txtTaskDesc.Text = _pendingTaskDesc;
            txtTaskDesc.ForeColor = TextDark;

            AddBotBubble(
                "Task added with the description \"" + _pendingTaskDesc +
                "\"\nWould you like a reminder? (e.g. 'Yes, remind me in 3 days' or 'No')");
        }

        private void HandleReminderResponse(string input)
        {
            _awaitingReminder = false;
            string lower = input.ToLower();

            string reminderText = null;
            DateTime? reminderDate = null;

            if (!lower.StartsWith("no") &&
                !lower.Equals("skip", StringComparison.OrdinalIgnoreCase))
            {
                int? days = _nlp.ExtractDays(input);
                if (days.HasValue)
                {
                    reminderDate = DateTime.Now.AddDays(days.Value);
                    reminderText = "In " + days.Value + " day" +
                                   (days.Value == 1 ? "" : "s") +
                                   " (" + reminderDate.Value.ToString("dd MMM yyyy") + ")";
                }
                else
                {
                    reminderText = input.Trim();
                    reminderDate = DateTime.Now.AddDays(7);
                }
                AddBotBubble("Got it! I'll remind you " + reminderText.ToLower() + ".");
            }
            else
            {
                AddBotBubble("No reminder set.");
            }

            TaskItem task = new TaskItem();
            task.Title = _pendingTaskTitle;
            task.Description = _pendingTaskDesc;
            task.Reminder = reminderText;
            task.ReminderDate = reminderDate;

            int id = _db.AddTask(task);
            _user.LogAction("Task saved: '" + _pendingTaskTitle + "'" +
                (reminderText != null ? " | " + reminderText : ""));

            lblStorage.Text = id > 0
                ? "Storage: MySQL (ID #" + id + ")"
                : "Storage: Local fallback, no DB";

            RefreshTaskList();
            _pendingTaskTitle = "";
            _pendingTaskDesc = "";
        }

        private void HandleViewTasks()
        {
            List<TaskItem> tasks = _db.GetAllTasks(reader);
            _user.LogAction("Viewed tasks");
            if (tasks.Count == 0)
            {
                AddBotBubble("No tasks yet. Use the panel on the right or type 'add task'.");
                return;
            }
            string msg = "Your tasks (" + tasks.Count + "):\n";
            foreach (TaskItem t in tasks)
                msg += "\n  " + (t.IsCompleted ? "[DONE]" : "[    ]") +
                       " #" + t.Id + "  " + t.Title;
            AddBotBubble(msg);
        }

        private void HandleCompleteTask(string input)
        {
            int id = ExtractId(input);
            if (id <= 0)
            {
                AddBotBubble("Which task? e.g. 'complete task #1'");
                return;
            }
            _db.MarkCompleted(id);
            _user.LogAction("Task #" + id + " completed");
            AddBotBubble("Task #" + id + " marked as completed.");
            RefreshTaskList();
        }

        private void HandleDeleteTask(string input)
        {
            int id = ExtractId(input);
            if (id <= 0)
            {
                AddBotBubble("Which task? e.g. 'delete task #2'");
                return;
            }
            _db.DeleteTask(id);
            _user.LogAction("Task #" + id + " deleted");
            AddBotBubble("Task #" + id + " deleted.");
            RefreshTaskList();
        }

        private void HandleSetReminder(string input)
        {
            int? days = _nlp.ExtractDays(input);
            string when = days.HasValue
                ? days.Value + " day(s) from now (" +
                  DateTime.Now.AddDays(days.Value).ToString("dd MMM yyyy") + ")"
                : "the specified time";
            _user.LogAction("Reminder: '" +
                input.Substring(0, Math.Min(input.Length, 40)) + "'");
            AddBotBubble("Reminder noted for " + when +
                ".\nType 'add task - [title]' to attach it to a task.");
        }

        private void HandleStartQuiz()
        {
            _quiz.Start();
            _user.LogAction("Quiz started");
            lblQuizScore.Text = "Score: 0 / " + _quiz.TotalQuestions;
            AddBotBubble(
                "Quiz started! " + _quiz.TotalQuestions +
                " questions. Use the buttons on the right panel to answer.");
            ShowQuizQuestion();
            SetQuizButtonsEnabled(true);
        }

        private void HandleShowActivityLog()
        {
            List<ActivityLogEntry> log = _user.GetRecentLog(10);
            _user.LogAction("Activity log viewed");
            string msg = "Here's a summary of recent actions:\n";
            for (int i = 0; i < log.Count; i++)
                msg += "\n  " + (i + 1) + ". " + log[i].Description;
            AddBotBubble(msg);
            RefreshLogView();
        }

        private void ShowHistory()
        {
            if (_user.QuestionHistory.Count == 0)
            {
                AddBotBubble("No history yet.");
                return;
            }
            string msg = "Question history:\n";
            for (int i = 0; i < _user.QuestionHistory.Count; i++)
                msg += "\n  " + (i + 1) + ". " + _user.QuestionHistory[i];
            AddBotBubble(msg);
        }

        // ══════════════════════════════════════════════════════════
        //  BUBBLE BUILDERS
        // ══════════════════════════════════════════════════════════
        private void AddBotBubble(string text)
        {
            Panel outer = new Panel();
            outer.Width = Math.Max(1, flpMessages.ClientSize.Width - 20);
            outer.AutoSize = true;
            outer.BackColor = Color.Transparent;
            outer.Padding = new Padding(0, 2, 40, 4);

            Label bubble = new Label();
            bubble.Text = text;
            bubble.Font = new Font("Segoe UI", 9.5f);
            bubble.ForeColor = TextDark;
            bubble.BackColor = BotMsgBg;
            bubble.AutoSize = true;
            bubble.MaximumSize = new Size(
                Math.Max(1, flpMessages.ClientSize.Width - 80), 0);
            bubble.Padding = new Padding(8, 6, 8, 6);
            bubble.BorderStyle = BorderStyle.FixedSingle;

            outer.Controls.Add(bubble);
            flpMessages.Controls.Add(outer);

            if (flpMessages.Controls.Count > 0)
                flpMessages.ScrollControlIntoView(
                    flpMessages.Controls[flpMessages.Controls.Count - 1]);
        }

        private void AddUserBubble(string text)
        {
            Panel outer = new Panel();
            outer.Width = Math.Max(1, flpMessages.ClientSize.Width - 20);
            outer.AutoSize = true;
            outer.BackColor = Color.Transparent;
            outer.Padding = new Padding(40, 2, 0, 4);

            Label bubble = new Label();
            bubble.Text = text;
            bubble.Font = new Font("Segoe UI", 9.5f);
            bubble.ForeColor = TextDark;
            bubble.BackColor = UserMsgBg;
            bubble.AutoSize = true;
            bubble.MaximumSize = new Size(
                Math.Max(1, flpMessages.ClientSize.Width - 80), 0);
            bubble.Padding = new Padding(8, 6, 8, 6);
            bubble.BorderStyle = BorderStyle.FixedSingle;

            outer.Controls.Add(bubble);
            flpMessages.Controls.Add(outer);

            if (flpMessages.Controls.Count > 0)
                flpMessages.ScrollControlIntoView(
                    flpMessages.Controls[flpMessages.Controls.Count - 1]);
        }

        // ══════════════════════════════════════════════════════════
        //  EVENT HANDLERS
        // ══════════════════════════════════════════════════════════
        private void BtnConnect_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                lblNameErr.Text = "Name cannot be empty.";
                return;
            }
            lblNameErr.Text = "";
            _user.Name = name;
            _nameSet = true;
            ShowMainScreen();
        }

        private void BtnAudio_Click(object sender, EventArgs e)
        {
            PlayGreeting();
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        private void TxtName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) BtnConnect_Click(sender, e);
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                SendMessage();
            }
        }

        private void TxtInput_GotFocus(object sender, EventArgs e)
        {
            if (txtInput.Text == PlaceholderInput)
            {
                txtInput.Text = "";
                txtInput.ForeColor = TextDark;
            }
        }

        private void TxtInput_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtInput.Text))
            {
                txtInput.Text = PlaceholderInput;
                txtInput.ForeColor = TextGray;
            }
        }

        private void TxtTaskTitle_GotFocus(object sender, EventArgs e)
        {
            if (txtTaskTitle.Text == PlaceholderTitle)
            {
                txtTaskTitle.Text = "";
                txtTaskTitle.ForeColor = TextDark;
            }
        }

        private void TxtTaskTitle_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTaskTitle.Text))
            {
                txtTaskTitle.Text = PlaceholderTitle;
                txtTaskTitle.ForeColor = TextGray;
            }
        }

        private void TxtTaskDesc_GotFocus(object sender, EventArgs e)
        {
            if (txtTaskDesc.Text == PlaceholderDesc)
            {
                txtTaskDesc.Text = "";
                txtTaskDesc.ForeColor = TextDark;
            }
        }

        private void TxtTaskDesc_LostFocus(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTaskDesc.Text))
            {
                txtTaskDesc.Text = PlaceholderDesc;
                txtTaskDesc.ForeColor = TextGray;
            }
        }

        // ══════════════════════════════════════════════════════════
        //  HELPERS
        // ══════════════════════════════════════════════════════════
        private static int ExtractId(string input)
        {
            Match match = Regex.Match(input, @"#?(\d+)");
            return match.Success ? int.Parse(match.Groups[1].Value) : -1;
        }

        private static string BuildDescription(string title)
        {
            string t = title.ToLower();
            if (t.Contains("2fa") || t.Contains("two-factor") || t.Contains("two factor"))
                return "Set up two-factor authentication to secure your accounts.";
            if (t.Contains("password"))
                return "Update and strengthen your passwords using a password manager.";
            if (t.Contains("privacy") || t.Contains("settings"))
                return "Review account privacy settings to ensure your data is protected.";
            if (t.Contains("backup"))
                return "Set up regular backups to protect against data loss or ransomware.";
            if (t.Contains("vpn"))
                return "Configure a VPN for secure browsing on public networks.";
            if (t.Contains("antivirus"))
                return "Install and update antivirus software to protect against malware.";
            if (t.Contains("firewall"))
                return "Enable and configure your firewall to block unauthorised access.";
            return char.ToUpper(title[0]) + title.Substring(1) +
                   " - complete this to improve your security posture.";
        }

        private Button MakeTabButton(string text)
        {
            Button b = new Button();
            b.Text = text;
            b.Font = new Font("Segoe UI", 9f);
            b.ForeColor = Color.White;
            b.BackColor = TabInactiveBg;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Width = 120;
            b.Height = 34;
            b.Cursor = Cursors.Hand;
            return b;
        }

        private Button MakeQuizOption(string letter)
        {
            Button b = new Button();
            b.Text = letter;
            b.Font = new Font("Segoe UI", 9f);
            b.ForeColor = TextDark;
            b.BackColor = Color.FromArgb(230, 230, 230);
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderColor = BorderGray;
            b.Dock = DockStyle.Top;
            b.Height = 30;
            b.Cursor = Cursors.Hand;
            b.Margin = new Padding(0, 0, 0, 3);
            return b;
        }

        private Panel WrapCenter(Control ctrl, int w, int h)
        {
            Panel p = new Panel();
            p.Dock = DockStyle.Fill;
            p.BackColor = Color.Transparent;
            p.Height = h + 10;
            p.Controls.Add(ctrl);
            ctrl.Tag = w;
            p.Resize += new EventHandler(WrapCenter_Resize);
            return p;
        }

        private void WrapCenter_Resize(object sender, EventArgs e)
        {
            Panel p = (Panel)sender;
            if (p.Controls.Count > 0)
            {
                Control c = p.Controls[0];
                int w = (int)c.Tag;
                c.Left = (p.Width - w) / 2;
            }
        }

        // ══════════════════════════════════════════════════════════
        //  DISPOSE
        // ══════════════════════════════════════════════════════════
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_soundPlayer != null)
            {
                _soundPlayer.Stop();
                _soundPlayer.Dispose();
            }
            base.OnFormClosing(e);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}