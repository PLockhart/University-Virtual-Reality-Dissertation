namespace MindWaveExperimentRecorder
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series7 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.eegChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.experimentLabel = new System.Windows.Forms.Label();
            this.participantLabel = new System.Windows.Forms.Label();
            this.exp1Button = new System.Windows.Forms.Button();
            this.exp2Button = new System.Windows.Forms.Button();
            this.exp3Button = new System.Windows.Forms.Button();
            this.fileSystemWatcher1 = new System.IO.FileSystemWatcher();
            this.recordingButton = new System.Windows.Forms.Button();
            this.participantField = new System.Windows.Forms.TextBox();
            this.newParticipantButton = new System.Windows.Forms.Button();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.experienceComboBox = new System.Windows.Forms.ComboBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.outputDirTextBox = new System.Windows.Forms.TextBox();
            this.baselineButton = new System.Windows.Forms.Button();
            this.exp1Checkbox = new System.Windows.Forms.CheckBox();
            this.exp2Checkbox = new System.Windows.Forms.CheckBox();
            this.exp3Checkbox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.eegChart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).BeginInit();
            this.SuspendLayout();
            // 
            // eegChart
            // 
            chartArea3.Name = "ChartArea1";
            this.eegChart.ChartAreas.Add(chartArea3);
            legend3.Name = "Legend1";
            this.eegChart.Legends.Add(legend3);
            this.eegChart.Location = new System.Drawing.Point(12, 137);
            this.eegChart.Name = "eegChart";
            series7.ChartArea = "ChartArea1";
            series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series7.Legend = "Legend1";
            series7.Name = "Attention";
            series8.ChartArea = "ChartArea1";
            series8.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series8.Legend = "Legend1";
            series8.Name = "Meditation";
            series9.ChartArea = "ChartArea1";
            series9.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.StepLine;
            series9.Legend = "Legend1";
            series9.Name = "BlinkStrength";
            this.eegChart.Series.Add(series7);
            this.eegChart.Series.Add(series8);
            this.eegChart.Series.Add(series9);
            this.eegChart.Size = new System.Drawing.Size(1083, 328);
            this.eegChart.TabIndex = 0;
            this.eegChart.Text = "chart1";
            // 
            // experimentLabel
            // 
            this.experimentLabel.AutoSize = true;
            this.experimentLabel.Location = new System.Drawing.Point(13, 13);
            this.experimentLabel.Name = "experimentLabel";
            this.experimentLabel.Size = new System.Drawing.Size(70, 13);
            this.experimentLabel.TabIndex = 1;
            this.experimentLabel.Text = "Experiment N";
            // 
            // participantLabel
            // 
            this.participantLabel.AutoSize = true;
            this.participantLabel.Location = new System.Drawing.Point(14, 30);
            this.participantLabel.Name = "participantLabel";
            this.participantLabel.Size = new System.Drawing.Size(57, 13);
            this.participantLabel.TabIndex = 2;
            this.participantLabel.Text = "Participant";
            // 
            // exp1Button
            // 
            this.exp1Button.Location = new System.Drawing.Point(976, 8);
            this.exp1Button.Name = "exp1Button";
            this.exp1Button.Size = new System.Drawing.Size(119, 23);
            this.exp1Button.TabIndex = 3;
            this.exp1Button.Text = "Experiment 1";
            this.exp1Button.UseVisualStyleBackColor = true;
            this.exp1Button.Click += new System.EventHandler(this.exp1Button_Click);
            // 
            // exp2Button
            // 
            this.exp2Button.Location = new System.Drawing.Point(976, 37);
            this.exp2Button.Name = "exp2Button";
            this.exp2Button.Size = new System.Drawing.Size(119, 23);
            this.exp2Button.TabIndex = 4;
            this.exp2Button.Text = "Experiment 2";
            this.exp2Button.UseVisualStyleBackColor = true;
            this.exp2Button.Click += new System.EventHandler(this.exp2Button_Click);
            // 
            // exp3Button
            // 
            this.exp3Button.Location = new System.Drawing.Point(976, 66);
            this.exp3Button.Name = "exp3Button";
            this.exp3Button.Size = new System.Drawing.Size(119, 23);
            this.exp3Button.TabIndex = 5;
            this.exp3Button.Text = "Experiment 3";
            this.exp3Button.UseVisualStyleBackColor = true;
            this.exp3Button.Click += new System.EventHandler(this.exp3Button_Click);
            // 
            // fileSystemWatcher1
            // 
            this.fileSystemWatcher1.EnableRaisingEvents = true;
            this.fileSystemWatcher1.SynchronizingObject = this;
            // 
            // recordingButton
            // 
            this.recordingButton.Location = new System.Drawing.Point(799, 8);
            this.recordingButton.Name = "recordingButton";
            this.recordingButton.Size = new System.Drawing.Size(161, 23);
            this.recordingButton.TabIndex = 6;
            this.recordingButton.Text = "Recording Button";
            this.recordingButton.UseVisualStyleBackColor = true;
            this.recordingButton.Click += new System.EventHandler(this.recordingButton_Click);
            // 
            // participantField
            // 
            this.participantField.Location = new System.Drawing.Point(799, 40);
            this.participantField.Name = "participantField";
            this.participantField.Size = new System.Drawing.Size(161, 20);
            this.participantField.TabIndex = 7;
            // 
            // newParticipantButton
            // 
            this.newParticipantButton.Location = new System.Drawing.Point(799, 93);
            this.newParticipantButton.Name = "newParticipantButton";
            this.newParticipantButton.Size = new System.Drawing.Size(161, 23);
            this.newParticipantButton.TabIndex = 8;
            this.newParticipantButton.Text = "Set New Participant";
            this.newParticipantButton.UseVisualStyleBackColor = true;
            this.newParticipantButton.Click += new System.EventHandler(this.newParticipantButton_Click);
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(139, 8);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(408, 105);
            this.logTextBox.TabIndex = 9;
            // 
            // experienceComboBox
            // 
            this.experienceComboBox.FormattingEnabled = true;
            this.experienceComboBox.Location = new System.Drawing.Point(799, 66);
            this.experienceComboBox.Name = "experienceComboBox";
            this.experienceComboBox.Size = new System.Drawing.Size(161, 21);
            this.experienceComboBox.TabIndex = 10;
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(976, 93);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(119, 23);
            this.saveButton.TabIndex = 11;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // outputDirTextBox
            // 
            this.outputDirTextBox.Location = new System.Drawing.Point(16, 66);
            this.outputDirTextBox.Name = "outputDirTextBox";
            this.outputDirTextBox.Size = new System.Drawing.Size(100, 20);
            this.outputDirTextBox.TabIndex = 12;
            this.outputDirTextBox.TextChanged += new System.EventHandler(this.outputDirTextBox_TextChanged);
            // 
            // baselineButton
            // 
            this.baselineButton.Location = new System.Drawing.Point(673, 8);
            this.baselineButton.Name = "baselineButton";
            this.baselineButton.Size = new System.Drawing.Size(102, 23);
            this.baselineButton.TabIndex = 13;
            this.baselineButton.Text = "Baseline";
            this.baselineButton.UseVisualStyleBackColor = true;
            this.baselineButton.Click += new System.EventHandler(this.button1_Click);
            // 
            // exp1Checkbox
            // 
            this.exp1Checkbox.AutoSize = true;
            this.exp1Checkbox.Location = new System.Drawing.Point(683, 40);
            this.exp1Checkbox.Name = "exp1Checkbox";
            this.exp1Checkbox.Size = new System.Drawing.Size(71, 17);
            this.exp1Checkbox.TabIndex = 14;
            this.exp1Checkbox.Text = "Exp 1 VR";
            this.exp1Checkbox.UseVisualStyleBackColor = true;
            // 
            // exp2Checkbox
            // 
            this.exp2Checkbox.AutoSize = true;
            this.exp2Checkbox.Location = new System.Drawing.Point(683, 69);
            this.exp2Checkbox.Name = "exp2Checkbox";
            this.exp2Checkbox.Size = new System.Drawing.Size(71, 17);
            this.exp2Checkbox.TabIndex = 15;
            this.exp2Checkbox.Text = "Exp 2 VR";
            this.exp2Checkbox.UseVisualStyleBackColor = true;
            // 
            // exp3Checkbox
            // 
            this.exp3Checkbox.AutoSize = true;
            this.exp3Checkbox.Location = new System.Drawing.Point(683, 99);
            this.exp3Checkbox.Name = "exp3Checkbox";
            this.exp3Checkbox.Size = new System.Drawing.Size(71, 17);
            this.exp3Checkbox.TabIndex = 16;
            this.exp3Checkbox.Text = "Exp 3 VR";
            this.exp3Checkbox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1107, 477);
            this.Controls.Add(this.exp3Checkbox);
            this.Controls.Add(this.exp2Checkbox);
            this.Controls.Add(this.exp1Checkbox);
            this.Controls.Add(this.baselineButton);
            this.Controls.Add(this.outputDirTextBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.experienceComboBox);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.newParticipantButton);
            this.Controls.Add(this.participantField);
            this.Controls.Add(this.recordingButton);
            this.Controls.Add(this.exp3Button);
            this.Controls.Add(this.exp2Button);
            this.Controls.Add(this.exp1Button);
            this.Controls.Add(this.participantLabel);
            this.Controls.Add(this.experimentLabel);
            this.Controls.Add(this.eegChart);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.eegChart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart eegChart;
        private System.Windows.Forms.Label experimentLabel;
        private System.Windows.Forms.Label participantLabel;
        private System.Windows.Forms.Button exp1Button;
        private System.Windows.Forms.Button exp2Button;
        private System.Windows.Forms.Button exp3Button;
        private System.IO.FileSystemWatcher fileSystemWatcher1;
        private System.Windows.Forms.Button recordingButton;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Button newParticipantButton;
        private System.Windows.Forms.TextBox participantField;
        private System.Windows.Forms.ComboBox experienceComboBox;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox outputDirTextBox;
        private System.Windows.Forms.Button baselineButton;
        private System.Windows.Forms.CheckBox exp3Checkbox;
        private System.Windows.Forms.CheckBox exp2Checkbox;
        private System.Windows.Forms.CheckBox exp1Checkbox;
    }
}

