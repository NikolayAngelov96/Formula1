using Formula1.Core.Contracts;
using Formula1.Models;
using Formula1.Models.Contracts;
using Formula1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Formula1.Core
{
    public class Controller : IController
    {
        private PilotRepository pilotRepository;
        private RaceRepository raceRepository;
        private FormulaOneCarRepository carRepository;

        public Controller()
        {
            this.pilotRepository = new PilotRepository();
            this.raceRepository = new RaceRepository();
            this.carRepository = new FormulaOneCarRepository();
        }

        public string AddCarToPilot(string pilotName, string carModel)
        {
            var car = this.carRepository.FindByName(carModel);
            var pilot = this.pilotRepository.FindByName(pilotName);

            if (pilot == null /*|| pilot.Car != null*/)
            {
                throw new InvalidOperationException($"Pilot {pilotName} does not exist or has a car.");
            }

            if (car == null)
            {
                throw new NullReferenceException($"Car {carModel} does not exist.");
            }

            pilot.AddCar(car);
            this.carRepository.Remove(car);

            return $"Pilot {pilotName} will drive a {car.GetType().Name} {carModel} car.";
        }

        public string AddPilotToRace(string raceName, string pilotFullName)
        {
            var race = this.raceRepository.FindByName(raceName);
            var pilot = this.pilotRepository.FindByName(pilotFullName);

            if (race == null)
            {
                throw new NullReferenceException($"Race {raceName} does not exist.");
            }

            if (pilot == null || pilot.CanRace == false || race.Pilots.Contains(pilot))
            {
                throw new InvalidOperationException($"Can not add pilot {pilotFullName} to the race.");
            }

            race.AddPilot(pilot);

            return $"Pilot {pilotFullName} is added to the {raceName} race.";
        }

        public string CreateCar(string type, string model, int horsepower, double engineDisplacement)
        {
            IFormulaOneCar car = this.carRepository.FindByName(model);

            if (car != null)
            {
                throw new InvalidOperationException($"Formula one car {model} is already created.");
            }

            if (type == "Ferrari")
            {
                var ferrariCar = new Ferrari(model, horsepower, engineDisplacement);
                this.carRepository.Add(ferrariCar);
            }
            else if (type == "Williams")
            {
                var williamsCar = new Williams(model, horsepower, engineDisplacement);
                this.carRepository.Add(williamsCar);
            }
            else
            {
                throw new InvalidOperationException($"Formula one car type {type} is not valid.");
            }

            return $"Car {type}, model {model} is created.";
        }

        public string CreatePilot(string fullName)
        {
            var pilot = this.pilotRepository.FindByName(fullName);

            if (pilot != null)
            {
                throw new InvalidOperationException($"Pilot {fullName} is already created.");
            }

            var newPilot = new Pilot(fullName);

            this.pilotRepository.Add(newPilot);
            return $"Pilot {fullName} is created.";
        }

        public string CreateRace(string raceName, int numberOfLaps)
        {
            var race = this.raceRepository.FindByName(raceName);

            if (race != null)
            {
                throw new InvalidOperationException($"Race {raceName} is already created.");
            }

            var newRace = new Race(raceName, numberOfLaps);

            this.raceRepository.Add(newRace);

            return $"Race {raceName} is created.";
        }

        public string PilotReport()
        {
            var sb = new StringBuilder();

            foreach (var pilot in this.pilotRepository.Models.OrderByDescending(x => x.NumberOfWins))
            {
                sb.AppendLine(pilot.ToString());
            }

            return sb.ToString().TrimEnd();
        }

        public string RaceReport()
        {
            var executedRaces = this.raceRepository.Models.Where(x => x.TookPlace == true);

            var sb = new StringBuilder();

            foreach (var race in executedRaces)
            {
                sb.AppendLine(race.RaceInfo());
            }

            return sb.ToString().TrimEnd();
        }

        public string StartRace(string raceName)
        {
            var race = this.raceRepository.FindByName(raceName);

            if (race == null)
            {
                throw new NullReferenceException($"Race { raceName } does not exist.");
            }

            if (race.Pilots.Count < 3)
            {
                throw new InvalidOperationException($"Race {raceName} cannot start with less than three participants.");
            }

            if (race.TookPlace == true)
            {
                throw new InvalidOperationException($"Can not execute race { raceName }.");
            }

            var winners = race.Pilots.OrderByDescending(x => x.Car.RaceScoreCalculator(race.NumberOfLaps)).ToList();

            race.TookPlace = true;


            winners[0].WinRace();

            var sb = new StringBuilder();

            sb.AppendLine($"Pilot {winners[0].FullName} wins the {raceName} race.");
            sb.AppendLine($"Pilot {winners[1].FullName} is second in the {raceName} race.");
            sb.AppendLine($"Pilot {winners[2].FullName} is third in the {raceName} race.");

            return sb.ToString().TrimEnd();
        }
    }
}
