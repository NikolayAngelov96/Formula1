﻿using Formula1.Models.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Formula1.Models
{
    public abstract class FormulaOneCar : IFormulaOneCar
    {
        private string model;
        private int horsePower;
        private double engineDisplacement;

        public FormulaOneCar(string model, int horsepower, double engineDisplacement)
        {
            this.Model = model;
            this.Horsepower = horsepower;
            this.EngineDisplacement = engineDisplacement;
        }

        public string Model
        {
            get => this.model;
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Length < 3)
                {
                    throw new ArgumentException($"Invalid car model: {value}.");
                }
                this.model = value;
            }
        }

        public int Horsepower
        {
            get => this.horsePower;
            set
            {
                if (value < 900 || value > 1050)
                {
                    throw new ArgumentException($"Invalid car horsepower: {value}.");
                }
                this.horsePower = value;
            }
        }

        public double EngineDisplacement
        {
            get => this.engineDisplacement;
            set
            {
                if (value < 1.6 || value > 2)
                {
                    throw new ArgumentException($"Invalid car engine displacement: {value}.");
                }
                this.engineDisplacement = value;
            }
        }

        public double RaceScoreCalculator(int laps)
        {
            return this.EngineDisplacement / this.Horsepower * laps;
        }
    }
}
