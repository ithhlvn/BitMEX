﻿namespace BitMEX.TestForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnMarketOrder = new System.Windows.Forms.Button();
            this.btnSwagger = new System.Windows.Forms.Button();
            this.btnLimitOrder = new System.Windows.Forms.Button();
            this.btnGetOrders = new System.Windows.Forms.Button();
            this.btnOpenAPI = new System.Windows.Forms.Button();
            this.NUDMarketOrderQuantity = new System.Windows.Forms.NumericUpDown();
            this.TBMarketOrder = new System.Windows.Forms.TextBox();
            this.NUDPrice = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.OutputLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMarketOrderQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDPrice)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.Controls.Add(this.btnMarketOrder, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSwagger, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.btnLimitOrder, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.btnGetOrders, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.btnOpenAPI, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.NUDMarketOrderQuantity, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.TBMarketOrder, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.NUDPrice, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.OutputLabel, 0, 6);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 10;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(776, 426);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btnMarketOrder
            // 
            this.btnMarketOrder.Location = new System.Drawing.Point(3, 3);
            this.btnMarketOrder.Name = "btnMarketOrder";
            this.btnMarketOrder.Size = new System.Drawing.Size(75, 23);
            this.btnMarketOrder.TabIndex = 0;
            this.btnMarketOrder.Text = "MarketOrder";
            this.btnMarketOrder.UseVisualStyleBackColor = true;
            this.btnMarketOrder.Click += new System.EventHandler(this.btnMarketOrder_Click);
            // 
            // btnSwagger
            // 
            this.btnSwagger.Location = new System.Drawing.Point(3, 45);
            this.btnSwagger.Name = "btnSwagger";
            this.btnSwagger.Size = new System.Drawing.Size(75, 23);
            this.btnSwagger.TabIndex = 1;
            this.btnSwagger.Text = "Swagger";
            this.btnSwagger.UseVisualStyleBackColor = true;
            this.btnSwagger.Click += new System.EventHandler(this.btnSwagger_Click);
            // 
            // btnLimitOrder
            // 
            this.btnLimitOrder.Location = new System.Drawing.Point(3, 87);
            this.btnLimitOrder.Name = "btnLimitOrder";
            this.btnLimitOrder.Size = new System.Drawing.Size(75, 23);
            this.btnLimitOrder.TabIndex = 2;
            this.btnLimitOrder.Text = "LimitOrder";
            this.btnLimitOrder.UseVisualStyleBackColor = true;
            this.btnLimitOrder.Click += new System.EventHandler(this.btnLimitOrder_Click);
            // 
            // btnGetOrders
            // 
            this.btnGetOrders.Location = new System.Drawing.Point(3, 129);
            this.btnGetOrders.Name = "btnGetOrders";
            this.btnGetOrders.Size = new System.Drawing.Size(75, 23);
            this.btnGetOrders.TabIndex = 3;
            this.btnGetOrders.Text = "GetOrders";
            this.btnGetOrders.UseVisualStyleBackColor = true;
            this.btnGetOrders.Click += new System.EventHandler(this.btnGetOrders_Click);
            // 
            // btnOpenAPI
            // 
            this.btnOpenAPI.Location = new System.Drawing.Point(3, 171);
            this.btnOpenAPI.Name = "btnOpenAPI";
            this.btnOpenAPI.Size = new System.Drawing.Size(75, 23);
            this.btnOpenAPI.TabIndex = 4;
            this.btnOpenAPI.Text = "OpenAPI";
            this.btnOpenAPI.UseVisualStyleBackColor = true;
            this.btnOpenAPI.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // NUDMarketOrderQuantity
            // 
            this.NUDMarketOrderQuantity.Location = new System.Drawing.Point(261, 45);
            this.NUDMarketOrderQuantity.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.NUDMarketOrderQuantity.Name = "NUDMarketOrderQuantity";
            this.NUDMarketOrderQuantity.Size = new System.Drawing.Size(91, 20);
            this.NUDMarketOrderQuantity.TabIndex = 8;
            // 
            // TBMarketOrder
            // 
            this.TBMarketOrder.Location = new System.Drawing.Point(261, 3);
            this.TBMarketOrder.Name = "TBMarketOrder";
            this.TBMarketOrder.Size = new System.Drawing.Size(91, 20);
            this.TBMarketOrder.TabIndex = 7;
            this.TBMarketOrder.Text = "XBTUSD";
            // 
            // NUDPrice
            // 
            this.NUDPrice.Increment = new decimal(new int[] {
            5,
            0,
            0,
            65536});
            this.NUDPrice.Location = new System.Drawing.Point(261, 87);
            this.NUDPrice.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.NUDPrice.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUDPrice.Name = "NUDPrice";
            this.NUDPrice.Size = new System.Drawing.Size(91, 20);
            this.NUDPrice.TabIndex = 10;
            this.NUDPrice.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(132, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 17);
            this.label1.TabIndex = 11;
            this.label1.Text = "SYMBOL >>";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(132, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 17);
            this.label2.TabIndex = 12;
            this.label2.Text = "QUANTITY >>";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(132, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "PRICE >>";
            // 
            // OutputLabel
            // 
            this.OutputLabel.AutoSize = true;
            this.OutputLabel.Location = new System.Drawing.Point(3, 252);
            this.OutputLabel.Name = "OutputLabel";
            this.OutputLabel.Size = new System.Drawing.Size(52, 13);
            this.OutputLabel.TabIndex = 9;
            this.OutputLabel.Text = "OUTPUT";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUDMarketOrderQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUDPrice)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button btnMarketOrder;
        private System.Windows.Forms.Button btnSwagger;
        private System.Windows.Forms.Button btnLimitOrder;
        private System.Windows.Forms.Button btnGetOrders;
        private System.Windows.Forms.Button btnOpenAPI;
        private System.Windows.Forms.TextBox TBMarketOrder;
        private System.Windows.Forms.NumericUpDown NUDMarketOrderQuantity;
        private System.Windows.Forms.Label OutputLabel;
        private System.Windows.Forms.NumericUpDown NUDPrice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

