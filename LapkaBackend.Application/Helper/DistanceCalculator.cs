﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapkaBackend.Application.Helper
{
    public class DistanceCalculator
    {
        private const double EarthRadiusKm = 6371.0; // Średni promień Ziemi

        // Funkcja do obliczania odległości między dwoma punktami na sferze
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c; // Odległość w kilometrach
        }

        // Funkcja do przeliczenia stopni na radiany
        private static double ToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }
    }
}
