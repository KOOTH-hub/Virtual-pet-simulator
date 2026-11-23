using System.Xml.Linq;

namespace Virtual_pet_simulator
{
    public partial class Form1 : Form
    {
        private XDocument? loadedXmlDocument;
        private readonly string saveFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VirtualPetSimulator",
            "pets.xml");
        private string petName = string.Empty;
        private int hunger = 50;
        private int happiness = 50;
        private int energy = 100;
        private int toilet = 50;

        public Form1()
        {
            InitializeComponent();
        }

        private XDocument LoadXmlFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The XML file could not be found.", filePath);
            }

            loadedXmlDocument = XDocument.Load(filePath);
            return loadedXmlDocument;
        }

        public void UpdatePetData(string petName, int hunger, int happiness, int energy, int toilet)
        {
            this.petName = petName;
            this.hunger = hunger;
            this.happiness = happiness;
            this.energy = energy;
            this.toilet = toilet;
            label1.Text = $"Pet Name: {petName}";
            label2.Text = $"Hunger: {hunger}";
            label3.Text = $"Happiness: {happiness}";
            label4.Text = $"Energy: {energy}";
            label5.Text = $"Toilet: {toilet}";
            label1.Refresh();
            label2.Refresh();
            label3.Refresh();
            label4.Refresh();
            label5.Refresh();
        }

        private void SavePetData(string petName, int hunger, int happiness, int energy, int toilet)
        {
            XDocument petDocument = new(
                new XElement("Pet",
                    new XElement("Name", petName),
                    new XElement("Hunger", hunger),
                    new XElement("Happiness", happiness),
                    new XElement("Energy", energy),
                    new XElement("Toilet", toilet)));

            string? directory = Path.GetDirectoryName(saveFilePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            petDocument.Save(saveFilePath);
            loadedXmlDocument = petDocument;
        }

        private void LoadPetData()
        {
            LoadXmlFile(saveFilePath);

            XElement pet = loadedXmlDocument?.Element("Pet")
                ?? throw new System.Xml.XmlException("The XML file does not contain a Pet element.");

            this.petName = pet.Element("Name")?.Value ?? string.Empty;

            if (!int.TryParse(pet.Element("Hunger")?.Value, out hunger) ||
                !int.TryParse(pet.Element("Happiness")?.Value, out happiness) ||
                !int.TryParse(pet.Element("Energy")?.Value, out energy) ||
                !int.TryParse(pet.Element("Toilet")?.Value, out toilet))
            {
                throw new System.Xml.XmlException("The pet values in the XML file are invalid.");
            }
            label1.Text = $"Pet Name: {petName}";
            label2.Text = $"Hunger: {hunger}";
            label3.Text = $"Happiness: {happiness}";
            label4.Text = $"Energy: {energy}";
            label5.Text = $"Toilet: {toilet}";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                LoadPetData();
            }
            catch (FileNotFoundException)
            {
                loadedXmlDocument = null;
                UpdatePetData(petName, hunger, happiness, energy, toilet);
            }
            catch (System.Xml.XmlException)
            {
                loadedXmlDocument = null;
                UpdatePetData(petName, hunger, happiness, energy, toilet);
                MessageBox.Show("The saved pet data is invalid. Starting with a new pet.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (IOException)
            {
                UpdatePetData(petName, hunger, happiness, energy, toilet);
                MessageBox.Show("The saved pet data could not be read. Starting with a new pet.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (UnauthorizedAccessException)
            {
                UpdatePetData(petName, hunger, happiness, energy, toilet);
                MessageBox.Show("The saved pet data could not be read. Starting with a new pet.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            TrySavePetData(false);
        }

        private void TrySavePetData(bool showMessage)
        {
            try
            {
                SavePetData(petName, hunger, happiness, energy, toilet);
                if (showMessage)
                {
                    MessageBox.Show("Pet stats saved.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (IOException)
            {
                if (showMessage)
                {
                    MessageBox.Show("The pet stats could not be saved.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (UnauthorizedAccessException)
            {
                if (showMessage)
                {
                    MessageBox.Show("The pet stats could not be saved.", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TrySavePetData(false);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            hunger = Math.Min(100, hunger + 1);
            UpdatePetData(petName, hunger, happiness, energy, toilet);
            if (hunger >= 100)
            {
                SetPetImage("Cat Hungry_Small.png");
            }
            else
            {
                SetPetImage("Cat Happy_Small.png");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hunger = Math.Max(0, hunger - 10);
            energy = Math.Min(150, energy + 5);
            toilet = Math.Min(200, toilet + 15);
            UpdatePetData(petName, hunger, happiness, energy, toilet);
            if (hunger >= 100)
            {
                SetPetImage("Cat Hungry_Small.png");
            }
            else
            {
                SetPetImage("Cat Happy_Small.png");
            }
        }

        private void SetPetImage(string imageName)
        {
            string imagePath = Path.Combine(AppContext.BaseDirectory, "Images", imageName);
            if (File.Exists(imagePath))
            {
                using Image image = Image.FromFile(imagePath);
                Image? previousImage = pictureBox1.Image;
                pictureBox1.Image = new Bitmap(image);
                previousImage?.Dispose();
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void button3_Click(object sender, EventArgs e)
        {
            TrySavePetData(true);
        }
    }
}
