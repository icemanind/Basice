namespace Basice.UI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtProgram = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.consoleProgram = new Basice.UI.ConsoleControl();
            this.label2 = new System.Windows.Forms.Label();
            this.BtnRunProgram = new System.Windows.Forms.Button();
            this.BtnLoadExample = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtProgram
            // 
            this.txtProgram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtProgram.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProgram.Location = new System.Drawing.Point(20, 37);
            this.txtProgram.Margin = new System.Windows.Forms.Padding(7);
            this.txtProgram.Multiline = true;
            this.txtProgram.Name = "txtProgram";
            this.txtProgram.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtProgram.Size = new System.Drawing.Size(871, 562);
            this.txtProgram.TabIndex = 0;
            this.txtProgram.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(333, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "Type your program here:";
            // 
            // consoleProgram
            // 
            this.consoleProgram.AllowInput = true;
            this.consoleProgram.BackColor = System.Drawing.Color.Black;
            this.consoleProgram.ConsoleBackgroundColor = System.Drawing.Color.Black;
            this.consoleProgram.ConsoleForegroundColor = System.Drawing.Color.LightGray;
            this.consoleProgram.CurrentBackgroundColor = System.Drawing.Color.Black;
            this.consoleProgram.CurrentForegroundColor = System.Drawing.Color.LightGray;
            this.consoleProgram.CursorType = Basice.UI.CursorTypes.Underline;
            this.consoleProgram.EchoInput = false;
            this.consoleProgram.ForeColor = System.Drawing.Color.LightGray;
            this.consoleProgram.Location = new System.Drawing.Point(901, 37);
            this.consoleProgram.Name = "consoleProgram";
            this.consoleProgram.ShowCursor = true;
            this.consoleProgram.Size = new System.Drawing.Size(646, 377);
            this.consoleProgram.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(1165, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(150, 18);
            this.label2.TabIndex = 3;
            this.label2.Text = "See the results here:";
            // 
            // BtnRunProgram
            // 
            this.BtnRunProgram.Location = new System.Drawing.Point(20, 609);
            this.BtnRunProgram.Name = "BtnRunProgram";
            this.BtnRunProgram.Size = new System.Drawing.Size(871, 36);
            this.BtnRunProgram.TabIndex = 4;
            this.BtnRunProgram.Text = "Run Program";
            this.BtnRunProgram.UseVisualStyleBackColor = true;
            this.BtnRunProgram.Click += new System.EventHandler(this.BtnRunProgram_Click);
            // 
            // BtnLoadExample
            // 
            this.BtnLoadExample.Location = new System.Drawing.Point(20, 651);
            this.BtnLoadExample.Name = "BtnLoadExample";
            this.BtnLoadExample.Size = new System.Drawing.Size(871, 36);
            this.BtnLoadExample.TabIndex = 5;
            this.BtnLoadExample.Text = "Load an Example!";
            this.BtnLoadExample.UseVisualStyleBackColor = true;
            this.BtnLoadExample.Click += new System.EventHandler(this.BtnLoadExample_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1571, 710);
            this.Controls.Add(this.BtnLoadExample);
            this.Controls.Add(this.BtnRunProgram);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.consoleProgram);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtProgram);
            this.Name = "MainForm";
            this.Text = "Basice Interpreter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtProgram;
        private System.Windows.Forms.Label label1;
        private ConsoleControl consoleProgram;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button BtnRunProgram;
        private System.Windows.Forms.Button BtnLoadExample;
    }
}

