namespace ChladniFigureSimulation
{
    public partial class MainPage : ContentPage
    {

        private ChladniView _chladniView;
        private Slider _frequencyXSlider;
        private Slider _frequencyYSlider;
        private Slider _particleCountSlider;
        private Label _freqXLabel;
        private Label _freqYLabel;
        private Label _particleLabel;
        private Picker _modePicker;

        public MainPage()
        {
            InitializeComponent();
            Title = "Chladni-Figuren Simulator";

            _chladniView = new ChladniView();

            _frequencyXSlider = new Slider
            {
                Minimum = 1,
                Maximum = 10,
                Value = 3,
                WidthRequest = 300
            };
            _frequencyXSlider.ValueChanged += OnFrequencyChanged;

            _frequencyYSlider = new Slider
            {
                Minimum = 1,
                Maximum = 10,
                Value = 2,
                WidthRequest = 300
            };
            _frequencyYSlider.ValueChanged += OnFrequencyChanged;

            _particleCountSlider = new Slider
            {
                Minimum = 100,
                Maximum = 2000,
                Value = 800,
                WidthRequest = 300
            };
            _particleCountSlider.ValueChanged += OnParticleCountChanged;

            _freqXLabel = new Label { Text = "Frequenz X: 3" };
            _freqYLabel = new Label { Text = "Frequenz Y: 2" };
            _particleLabel = new Label { Text = "Partikel: 800" };

            _modePicker = new Picker
            {
                Title = "Schwingungsmodus",
                ItemsSource = new List<string> { "Standard", "Circular", "Radial", "Complex" },
                SelectedIndex = 0
            };
            _modePicker.SelectedIndexChanged += OnModeChanged;

            var resetButton = new Button
            {
                Text = "Zurücksetzen",
                WidthRequest = 150
            };
            resetButton.Clicked += (s, e) => _chladniView.ResetParticles();

            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children =
                {
                    new Label
                    {
                        Text = "Chladni-Figuren Simulator",
                        FontSize = 24,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new Frame
                    {
                        Content = _chladniView,
                        HeightRequest = 400,
                        WidthRequest = 400,
                        HorizontalOptions = LayoutOptions.Center,
                        CornerRadius = 10,
                        HasShadow = true
                    },
                    _freqXLabel,
                    _frequencyXSlider,
                    _freqYLabel,
                    _frequencyYSlider,
                    _particleLabel,
                    _particleCountSlider,
                    _modePicker,
                    resetButton
                }
            };

            _chladniView.SetFrequencies((int)_frequencyXSlider.Value, (int)_frequencyYSlider.Value);
        }



        private void OnFrequencyChanged(object sender, ValueChangedEventArgs e)
        {
            int freqX = (int)_frequencyXSlider.Value;
            int freqY = (int)_frequencyYSlider.Value;
            _freqXLabel.Text = $"Frequenz X: {freqX}";
            _freqYLabel.Text = $"Frequenz Y: {freqY}";
            _chladniView.SetFrequencies(freqX, freqY);
        }

        private void OnParticleCountChanged(object sender, ValueChangedEventArgs e)
        {
            int count = (int)_particleCountSlider.Value;
            _particleLabel.Text = $"Partikel: {count}";
            _chladniView.SetParticleCount(count);
        }

        private void OnModeChanged(object sender, EventArgs e)
        {
            _chladniView.SetMode(_modePicker.SelectedIndex);
        }


    }

    public class ChladniView : GraphicsView
    {
        private ChladniDrawable _drawable;

        public ChladniView()
        {
            _drawable = new ChladniDrawable();
            Drawable = _drawable;
            StartAnimation();
        }

        private void StartAnimation()
        {
            Dispatcher.StartTimer(TimeSpan.FromMilliseconds(33), () =>
            {
                _drawable.Update();
                Invalidate();
                return true;
            });
        }

        public void SetFrequencies(int freqX, int freqY)
        {
            _drawable.SetFrequencies(freqX, freqY);
        }

        public void SetParticleCount(int count)
        {
            _drawable.SetParticleCount(count);
        }

        public void ResetParticles()
        {
            _drawable.ResetParticles();
        }

        public void SetMode(int mode)
        {
            _drawable.SetMode(mode);
        }
    }

    public class Particle
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float VelocityX { get; set; }
        public float VelocityY { get; set; }
    }

    public class ChladniDrawable : IDrawable
    {
        private List<Particle> _particles;
        private int _freqX = 3;
        private int _freqY = 2;
        private float _time = 0;
        private int _particleCount = 800;
        private int _mode = 0;
        private Random _random = new Random();

        public ChladniDrawable()
        {
            InitializeParticles();
        }

        private void InitializeParticles()
        {
            _particles = new List<Particle>();
            for (int i = 0; i < _particleCount; i++)
            {
                _particles.Add(new Particle
                {
                    X = (float)_random.NextDouble(),
                    Y = (float)_random.NextDouble(),
                    VelocityX = 0,
                    VelocityY = 0
                });
            }
        }

        public void SetFrequencies(int freqX, int freqY)
        {
            _freqX = freqX;
            _freqY = freqY;
        }

        public void SetParticleCount(int count)
        {
            _particleCount = count;
            InitializeParticles();
        }

        public void ResetParticles()
        {
            InitializeParticles();
        }

        public void SetMode(int mode)
        {
            _mode = mode;
        }

        private float CalculateAmplitude(float x, float y)
        {
            switch (_mode)
            {
                case 0: // Standard
                    return (float)(Math.Sin(_freqX * Math.PI * x) * Math.Sin(_freqY * Math.PI * y));

                case 1: // Circular
                    float r = (float)Math.Sqrt((x - 0.5) * (x - 0.5) + (y - 0.5) * (y - 0.5)) * 2;
                    float theta = (float)Math.Atan2(y - 0.5, x - 0.5);
                    return (float)(Math.Sin(_freqX * Math.PI * r) * Math.Cos(_freqY * theta));

                case 2: // Radial
                    float radius = (float)Math.Sqrt((x - 0.5) * (x - 0.5) + (y - 0.5) * (y - 0.5)) * 2;
                    return (float)Math.Sin(_freqX * Math.PI * radius) * (float)Math.Sin(_freqY * Math.PI * radius);

                case 3: // Complex
                    return (float)(Math.Sin(_freqX * Math.PI * x) * Math.Sin(_freqY * Math.PI * y) +
                                   0.5 * Math.Sin((_freqX + 1) * Math.PI * x) * Math.Sin((_freqY - 1) * Math.PI * y));

                default:
                    return 0;
            }
        }

        public void Update()
        {
            _time += 0.05f;
            float damping = 0.95f;
            float forceScale = 0.002f;

            foreach (var particle in _particles)
            {
                // Berechne die Amplitude an der aktuellen Position
                float amplitude = CalculateAmplitude(particle.X, particle.Y);

                // Berechne den Gradienten (Kraft) durch numerische Differentiation
                float dx = 0.01f;
                float dy = 0.01f;
                float ampXPlus = CalculateAmplitude(particle.X + dx, particle.Y);
                float ampXMinus = CalculateAmplitude(particle.X - dx, particle.Y);
                float ampYPlus = CalculateAmplitude(particle.X, particle.Y + dy);
                float ampYMinus = CalculateAmplitude(particle.X, particle.Y - dy);

                float forceX = -(ampXPlus - ampXMinus) / (2 * dx);
                float forceY = -(ampYPlus - ampYMinus) / (2 * dy);

                // Aktualisiere Geschwindigkeit
                particle.VelocityX += forceX * forceScale;
                particle.VelocityY += forceY * forceScale;

                // Dämpfung
                particle.VelocityX *= damping;
                particle.VelocityY *= damping;

                // Aktualisiere Position
                particle.X += particle.VelocityX;
                particle.Y += particle.VelocityY;

                // Begrenze auf [0, 1]
                particle.X = Math.Max(0, Math.Min(1, particle.X));
                particle.Y = Math.Max(0, Math.Min(1, particle.Y));
            }
        }

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = Colors.Black;
            canvas.FillRectangle(dirtyRect);

            float width = dirtyRect.Width;
            float height = dirtyRect.Height;
            float size = Math.Min(width, height);
            float offsetX = (width - size) / 2;
            float offsetY = (height - size) / 2;

            // Zeichne schwache Gitterlinien für die Membran
            canvas.StrokeColor = Colors.DarkGray;
            canvas.StrokeSize = 1;
            for (int i = 0; i <= 10; i++)
            {
                float pos = offsetX + i * size / 10;
                canvas.DrawLine(pos, offsetY, pos, offsetY + size);
                pos = offsetY + i * size / 10;
                canvas.DrawLine(offsetX, pos, offsetX + size, pos);
            }

            // Zeichne Rahmen
            canvas.StrokeColor = Colors.Gray;
            canvas.StrokeSize = 2;
            canvas.DrawRectangle(offsetX, offsetY, size, size);

            // Zeichne Partikel (Eisenspäne)
            canvas.FillColor = Colors.LightGray;
            foreach (var particle in _particles)
            {
                float x = offsetX + particle.X * size;
                float y = offsetY + particle.Y * size;
                canvas.FillCircle(x, y, 1.5f);
            }

            // Info-Text
            canvas.FontColor = Colors.White;
            canvas.FontSize = 12;
            canvas.DrawString($"Modus: {GetModeName()} | Freq: {_freqX}×{_freqY}",
                            10, height - 20, HorizontalAlignment.Left);
        }

        private string GetModeName()
        {
            return _mode switch
            {
                0 => "Standard",
                1 => "Circular",
                2 => "Radial",
                3 => "Complex",
                _ => "Unknown"
            };
        }
    }
}
