using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
namespace Books
{
    public partial class Form1 : Form
    {
        private List<Book> books = new List<Book>();
        private List<Reader> readers = new List<Reader>();
        private List<BorrowRecord> borrowRecords = new List<BorrowRecord>();

        public Form1()
        {
            InitializeComponent();
            InitializeSampleData();
            RefreshDataGrids();
        }
        private void InitializeSampleData()
        {
            // Добавляем примеры книг
            books.Add(new Book(1, "Война и мир", "Лев Толстой", "Русская классика", "978-5-389-06256-6", 1869));
            books.Add(new Book(2, "1984", "Джордж Оруэлл", "Антиутопия", "978-5-17-090640-3", 1949));
            books.Add(new Book(3, "Мастер и Маргарита", "Михаил Булгаков", "Русская классика", "978-5-17-090639-7", 1967));
            books.Add(new Book(4, "Преступление и наказание", "Федор Достоевский", "Русская классика", "978-5-389-06257-3", 1866));

            // Добавляем примеры читателей
            readers.Add(new Reader(1, "Иванов Иван Иванович", "ivanov@example.com", "+7 (123) 456-7890"));
            readers.Add(new Reader(2, "Петрова Мария Сергеевна", "petrova@example.com", "+7 (987) 654-3210"));
        }

        private void RefreshDataGrids()
        {
            // Обновляем DataGridView с книгами
            dgvBooks.DataSource = null;
            dgvBooks.DataSource = books.Select(b => new
            {
                b.Id,
                b.Title,
                b.Author,
                b.Genre,
                b.ISBN,
                b.Year,
                Status = b.IsAvailable ? "Доступна" : "Выдана"
            }).ToList();

            // Обновляем DataGridView с читателями
            dgvReaders.DataSource = null;
            dgvReaders.DataSource = readers;

            // Обновляем DataGridView с записями о выдаче
            dgvBorrowRecords.DataSource = null;
            dgvBorrowRecords.DataSource = borrowRecords.Select(br => new
            {
                br.Id,
                BookTitle = books.FirstOrDefault(b => b.Id == br.BookId)?.Title,
                ReaderName = readers.FirstOrDefault(r => r.Id == br.ReaderId)?.Name,
                br.BorrowDate,
                br.DueDate,
                br.ReturnDate,
                Status = br.ReturnDate.HasValue ? "Возвращена" : "На руках"
            }).ToList();
        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            using (var form = new BookForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int newId = books.Count > 0 ? books.Max(b => b.Id) + 1 : 1;
                    books.Add(new Book(
                        newId,
                        form.BookTitle,
                        form.Author,
                        form.Genre,
                        form.ISBN,
                        form.Year
                    ));
                    RefreshDataGrids();
                }
            }
        }

        private void btnEditBook_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0) return;

            int bookId = (int)dgvBooks.SelectedRows[0].Cells["Id"].Value;
            Book book = books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return;

            using (var form = new BookForm(book))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    book.Title = form.BookTitle;
                    book.Author = form.Author;
                    book.Genre = form.Genre;
                    book.ISBN = form.ISBN;
                    book.Year = form.Year;
                    RefreshDataGrids();
                }
            }
        }

        private void btnDeleteBook_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0) return;

            int bookId = (int)dgvBooks.SelectedRows[0].Cells["Id"].Value;
            if (borrowRecords.Any(br => br.BookId == bookId && !br.ReturnDate.HasValue))
            {
                MessageBox.Show("Нельзя удалить книгу, которая сейчас выдана читателю!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить эту книгу?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                books.RemoveAll(b => b.Id == bookId);
                borrowRecords.RemoveAll(br => br.BookId == bookId);
                RefreshDataGrids();
            }
        }

        private void btnAddReader_Click(object sender, EventArgs e)
        {
            using (var form = new ReaderForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int newId = readers.Count > 0 ? readers.Max(r => r.Id) + 1 : 1;
                    readers.Add(new Reader(
                        newId,
                        form.ReaderName,
                        form.Email,
                        form.Phone
                    ));
                    RefreshDataGrids();
                }
            }
        }

        private void btnEditReader_Click(object sender, EventArgs e)
        {
            if (dgvReaders.SelectedRows.Count == 0) return;

            int readerId = (int)dgvReaders.SelectedRows[0].Cells["Id"].Value;
            Reader reader = readers.FirstOrDefault(r => r.Id == readerId);
            if (reader == null) return;

            using (var form = new ReaderForm(reader))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    reader.Name = form.ReaderName;
                    reader.Email = form.Email;
                    reader.Phone = form.Phone;
                    RefreshDataGrids();
                }
            }
        }

        private void btnDeleteReader_Click(object sender, EventArgs e)
        {
            if (dgvReaders.SelectedRows.Count == 0) return;

            int readerId = (int)dgvReaders.SelectedRows[0].Cells["Id"].Value;
            if (borrowRecords.Any(br => br.ReaderId == readerId && !br.ReturnDate.HasValue))
            {
                MessageBox.Show("Нельзя удалить читателя, у которого есть не возвращенные книги!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Вы уверены, что хотите удалить этого читателя?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                readers.RemoveAll(r => r.Id == readerId);
                borrowRecords.RemoveAll(br => br.ReaderId == readerId);
                RefreshDataGrids();
            }
        }

        private void btnBorrowBook_Click(object sender, EventArgs e)
        {
            if (dgvBooks.SelectedRows.Count == 0 || dgvReaders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Пожалуйста, выберите и книгу, и читателя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int bookId = (int)dgvBooks.SelectedRows[0].Cells["Id"].Value;
            int readerId = (int)dgvReaders.SelectedRows[0].Cells["Id"].Value;

            Book book = books.FirstOrDefault(b => b.Id == bookId);
            if (book == null) return;

            if (!book.IsAvailable)
            {
                MessageBox.Show("Эта книга уже выдана другому читателю", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var form = new BorrowForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    int newId = borrowRecords.Count > 0 ? borrowRecords.Max(br => br.Id) + 1 : 1;
                    borrowRecords.Add(new BorrowRecord(
                        newId,
                        bookId,
                        readerId,
                        form.BorrowDate,
                        form.DueDate
                    ));
                    book.IsAvailable = false;
                    RefreshDataGrids();
                }
            }
        }

        private void btnReturnBook_Click(object sender, EventArgs e)
        {
            if (dgvBorrowRecords.SelectedRows.Count == 0) return;

            int recordId = (int)dgvBorrowRecords.SelectedRows[0].Cells["Id"].Value;
            BorrowRecord record = borrowRecords.FirstOrDefault(br => br.Id == recordId);
            if (record == null || record.ReturnDate.HasValue) return;

            record.ReturnDate = DateTime.Now;
            Book book = books.FirstOrDefault(b => b.Id == record.BookId);
            if (book != null) book.IsAvailable = true;

            RefreshDataGrids();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();
            var filteredBooks = books.Where(b =>
                b.Title.ToLower().Contains(searchTerm) ||
                b.Author.ToLower().Contains(searchTerm) ||
                b.Genre.ToLower().Contains(searchTerm) ||
                b.ISBN.ToLower().Contains(searchTerm) ||
                b.Year.ToString().Contains(searchTerm)
            ).ToList();

            dgvBooks.DataSource = null;
            dgvBooks.DataSource = filteredBooks.Select(b => new
            {
                b.Id,
                b.Title,
                b.Author,
                b.Genre,
                b.ISBN,
                b.Year,
                Status = b.IsAvailable ? "Доступна" : "Выдана"
            }).ToList();
        }

        private void btnClearSearch_Click(object sender, EventArgs e)
        {
            txtSearch.Text = "";
            RefreshDataGrids();
        }

        private void btnFilterAvailable_Click(object sender, EventArgs e)
        {
            dgvBooks.DataSource = null;
            dgvBooks.DataSource = books.Where(b => b.IsAvailable).Select(b => new
            {
                b.Id,
                b.Title,
                b.Author,
                b.Genre,
                b.ISBN,
                b.Year,
                Status = "Доступна"
            }).ToList();
        }

        private void btnFilterBorrowed_Click(object sender, EventArgs e)
        {
            dgvBooks.DataSource = null;
            dgvBooks.DataSource = books.Where(b => !b.IsAvailable).Select(b => new
            {
                b.Id,
                b.Title,
                b.Author,
                b.Genre,
                b.ISBN,
                b.Year,
                Status = "Выдана"
            }).ToList();
        }
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string ISBN { get; set; }
        public int Year { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Book(int id, string title, string author, string genre, string isbn, int year)
        {
            Id = id;
            Title = title;
            Author = author;
            Genre = genre;
            ISBN = isbn;
            Year = year;
        }
    }

    public class Reader
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Reader(int id, string name, string email, string phone)
        {
            Id = id;
            Name = name;
            Email = email;
            Phone = phone;
        }
    }

    public class BorrowRecord
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int ReaderId { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public BorrowRecord(int id, int bookId, int readerId, DateTime borrowDate, DateTime dueDate)
        {
            Id = id;
            BookId = bookId;
            ReaderId = readerId;
            BorrowDate = borrowDate;
            DueDate = dueDate;
        }
    }

    public class BookForm : Form
    {
        private TextBox txtTitle;
        private TextBox txtAuthor;
        private TextBox txtGenre;
        private TextBox txtISBN;
        private NumericUpDown nudYear;
        private Button btnOK;
        private Button btnCancel;

        public string BookTitle => txtTitle.Text;
        public string Author => txtAuthor.Text;
        public string Genre => txtGenre.Text;
        public string ISBN => txtISBN.Text;
        public int Year => (int)nudYear.Value;

        public BookForm(Book book = null)
        {
            InitializeComponents();
            if (book != null)
            {
                txtTitle.Text = book.Title;
                txtAuthor.Text = book.Author;
                txtGenre.Text = book.Genre;
                txtISBN.Text = book.ISBN;
                nudYear.Value = book.Year;
            }
        }

        private void InitializeComponents()
        {
            this.Text = "Добавить/Редактировать книгу";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblTitle = new Label { Text = "Название:", Left = 20, Top = 20, Width = 100 };
            txtTitle = new TextBox { Left = 130, Top = 20, Width = 240 };

            Label lblAuthor = new Label { Text = "Автор:", Left = 20, Top = 50, Width = 100 };
            txtAuthor = new TextBox { Left = 130, Top = 50, Width = 240 };

            Label lblGenre = new Label { Text = "Жанр:", Left = 20, Top = 80, Width = 100 };
            txtGenre = new TextBox { Left = 130, Top = 80, Width = 240 };

            Label lblISBN = new Label { Text = "ISBN:", Left = 20, Top = 110, Width = 100 };
            txtISBN = new TextBox { Left = 130, Top = 110, Width = 240 };

            Label lblYear = new Label { Text = "Год издания:", Left = 20, Top = 140, Width = 100 };
            nudYear = new NumericUpDown { Left = 130, Top = 140, Width = 100, Minimum = 0, Maximum = DateTime.Now.Year, Value = DateTime.Now.Year };

            btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 130, Top = 180, Width = 100 };
            btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Left = 240, Top = 180, Width = 100 };

            btnOK.Click += (sender, e) => { if (ValidateForm()) this.DialogResult = DialogResult.OK; };
            btnCancel.Click += (sender, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { lblTitle, txtTitle, lblAuthor, txtAuthor, lblGenre, txtGenre, lblISBN, txtISBN, lblYear, nudYear, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Пожалуйста, введите название книги", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtAuthor.Text))
            {
                MessageBox.Show("Пожалуйста, введите автора книги", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }

    public class ReaderForm : Form
    {
        private TextBox txtName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private Button btnOK;
        private Button btnCancel;

        public string ReaderName => txtName.Text;
        public string Email => txtEmail.Text;
        public string Phone => txtPhone.Text;

        public ReaderForm(Reader reader = null)
        {
            InitializeComponents();
            if (reader != null)
            {
                txtName.Text = reader.Name;
                txtEmail.Text = reader.Email;
                txtPhone.Text = reader.Phone;
            }
        }

        private void InitializeComponents()
        {
            this.Text = "Добавить/Редактировать читателя";
            this.Size = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblName = new Label { Text = "ФИО:", Left = 20, Top = 20, Width = 100 };
            txtName = new TextBox { Left = 130, Top = 20, Width = 240 };

            Label lblEmail = new Label { Text = "Email:", Left = 20, Top = 50, Width = 100 };
            txtEmail = new TextBox { Left = 130, Top = 50, Width = 240 };

            Label lblPhone = new Label { Text = "Телефон:", Left = 20, Top = 80, Width = 100 };
            txtPhone = new TextBox { Left = 130, Top = 80, Width = 240 };

            btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 130, Top = 120, Width = 100 };
            btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Left = 240, Top = 120, Width = 100 };

            btnOK.Click += (sender, e) => { if (ValidateForm()) this.DialogResult = DialogResult.OK; };
            btnCancel.Click += (sender, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[] { lblName, txtName, lblEmail, txtEmail, lblPhone, txtPhone, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Пожалуйста, введите ФИО читателя", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }
    }

    public class BorrowForm : Form
    {
        private DateTimePicker dtpBorrowDate;
        private DateTimePicker dtpDueDate;
        private Button btnOK;
        private Button btnCancel;

        public DateTime BorrowDate => dtpBorrowDate.Value;
        public DateTime DueDate => dtpDueDate.Value;

        public BorrowForm()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Text = "Выдача книги";
            this.Size = new Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            Label lblBorrowDate = new Label { Text = "Дата выдачи:", Left = 20, Top = 20, Width = 100 };
            dtpBorrowDate = new DateTimePicker { Left = 130, Top = 20, Width = 150, Value = DateTime.Now };

            Label lblDueDate = new Label { Text = "Дата возврата:", Left = 20, Top = 50, Width = 100 };
            dtpDueDate = new DateTimePicker { Left = 130, Top = 50, Width = 150, Value = DateTime.Now.AddDays(14) };

            btnOK = new Button { Text = "OK", DialogResult = DialogResult.OK, Left = 50, Top = 80, Width = 100 };
            btnCancel = new Button { Text = "Отмена", DialogResult = DialogResult.Cancel, Left = 160, Top = 80, Width = 100 };

            this.Controls.AddRange(new Control[] { lblBorrowDate, dtpBorrowDate, lblDueDate, dtpDueDate, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }
    }
}
