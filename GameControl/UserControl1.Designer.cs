namespace GameControl
{
    partial class GameControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // GameControl
            // 
            this.AutoScroll = true;
            this.Name = "GameControl";
            this.Size = new System.Drawing.Size(301, 301);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GameControl_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.GameControl_MouseClick);
            this.MouseLeave += new System.EventHandler(this.GameControl_MouseLeave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GameControl_MouseMove);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.GameControl_MouseWheel);
            this.Resize += new System.EventHandler(this.GameControl_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
