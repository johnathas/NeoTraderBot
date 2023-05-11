﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Runtime.Script;
using TradeApi;
using TradeApi.History;
using TradeApi.Indicators;
using TradeApi.Instruments;
using TradeApi.ToolBelt;


namespace NeoTraderBot
{
    /// <summary>
    /// VolatilidadeMedia
    /// 
    /// </summary>
    public class NTB_VolatilidadeMedia : IndicatorBuilder 
    {
        public NTB_VolatilidadeMedia()
            : base()
        {
			#region Initialization
            Credentials.Author = "Johnathas Carvalho";
            Credentials.Company = "Comunidade NeoTraderBot";
            Credentials.Copyrights = "Comunidade NeoTraderBot";
            Credentials.DateOfCreation = new DateTime(2023, 5, 11);
            Credentials.ExpirationDate = DateTime.MinValue;
            Credentials.Version = "V1.0";
            Credentials.Password = "66b4a6416f59370e942d353f08a9ae36";
            Credentials.ProjectName = "VolatilidadeMedia";
            #endregion 
            
			Lines.Set("Volat.");
            Lines.Set("Média Volat.");
			
			
            Lines["Volat."].Color = Color.FromArgb(52,52,52);
			Lines["Volat."].Style = LineStyle.Histogram;
			
			Lines["Média Volat."].Color = Color.WhiteSmoke;
			Lines["Média Volat."].Style = LineStyle.DotLine;
			

            SeparateWindow = true;
        }
        
        
        #region input
		[InputParameter(InputType.Numeric, "Qtde de periodos", 0)]
		[SimpleNumeric(1D,9999D)]
		public int Periodos = 100;
        
		[InputParameterAttribute(InputType.Combobox, "Tipo da média", 1)]
		[ComboboxItem("Aritmética (SMA)", MAMode.SMA)]
		[ComboboxItem("Exponencial (EMA)", MAMode.EMA)]
		[ComboboxItem("Amortecida (SMMA)", MAMode.SMMA)]		
		[ComboboxItem("Ponderada Linear (LWMA)", MAMode.LWMA)]		
		public MAMode TipoMedia = MAMode.SMA;
        #endregion
       
        #region variables
		private BarData barData;
		private BuiltInIndicator mediaVolat;
		
		private Color corAlta;
		private Color corBaixa;
        #endregion        
        
        /// <summary>
        /// This function will be called after creating
        /// </summary>
		public override void Init()
		{
			ScriptShortName = (string.Format("Volat. Média ({0},{1})", Periodos, TipoMedia));
			barData = HistoryDataSeries as BarData;
			mediaVolat = IndicatorsManager.BuildIn.MA((x) => Range(x), Periodos, TipoMedia);
			
			Lines["Volat."].Visible = true;
			Lines["Média Volat."].Visible = true;

			corAlta = Color.FromArgb(0, 51, 0);
			corBaixa = Color.FromArgb(153, 0, 0);
			Lines["Volat."].Width = 100;		
		}        
 
        /// <summary>
        /// Entry point. This function is called when new quote comes or new bar created
        /// </summary>
        public override void Update(TickStatus args)
        {
        	if (args == TickStatus.IsQuote)
        	{
				if (barData.Count < Periodos)
					return;
				
				double rangeAtual = Range(0);
				double media = mediaVolat.GetValue();
				
				Lines["Volat."].SetValue(rangeAtual);
				Lines["Média Volat."].SetValue(media);
				
				if (rangeAtual >= mediaVolat.GetValue())
				{
					Lines["Volat."].Markers.Set((barData.GetOpen() < barData.GetClose()) ? corAlta : corBaixa, 0);
				}
        	}
        }
        
        /// <summary>
        /// This function will be called before removing
        /// </summary>
		public override void Complete()
		{
			
		} 
		
		#region customMethods
		private double Range(int index){
			return this.barData.GetHigh(index) - this.barData.GetLow(index);
		}
		
		#endregion		
		
		
     }
}
