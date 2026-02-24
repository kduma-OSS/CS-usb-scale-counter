using System;
using HidClient;
using UnitsNet;

namespace ScaleLib
{
    public class Scale: AbstractHidClient
    {
        private static readonly Mass WeightChangeTolerance = Mass.FromOunces(0.05);

        protected override int VendorId { get; } = 0x0922;
        protected override int ProductId { get; } = 0x8003;

        private Mass _weight;
        private StatusEnum _status;

        public event EventHandler<Mass> WeightChanged;
        public event EventHandler<StatusEnum> StatusChanged;
        public Mass Weight
        {
            get => _weight;
            internal set
            {
                var isChange = !_weight.Equals(value, WeightChangeTolerance);

                if (!isChange) return;

                _weight = value;
                EventSynchronizationContext.Post(OnWeightChanged, _weight);
            }
        }
        public StatusEnum Status
        {
            get => _status;
            internal set
            {
                var isChange = !_status.Equals(value);

                if (!isChange) return;

                _status = value;
                EventSynchronizationContext.Post(OnStatusChanged, _status);
            }
        }

        private void OnWeightChanged(object weight)
        {
            WeightChanged?.Invoke(this, (Mass)weight);

            OnPropertyChanged();
        }

        private void OnStatusChanged(object status)
        {
            StatusChanged?.Invoke(this, (StatusEnum)status);

            OnPropertyChanged();
        }

        protected override void OnHidRead(byte[] readBuffer)
        {
            if (readBuffer.Length < 6 || readBuffer[0] != 3)
            {
                Console.WriteLine(@"[{0}]", string.Join(", ", readBuffer));

                return;
            }

            Status = MapStatus(readBuffer[1]);

            int exponent = unchecked((sbyte)readBuffer[3]);
            double baseValue = readBuffer[4] + (readBuffer[5] * 256);
            double weight = baseValue * Math.Pow(10, exponent);


            switch (readBuffer[2])
            {
                case 11: // Ounces
                    Weight = Mass.FromOunces(weight);
                    break;

                case 2:  // Grams
                default:
                    Weight = Mass.FromGrams(weight);
                    break;
            }
        }

        private static StatusEnum MapStatus(byte statusByte)
        {
            switch (statusByte)
            {
                case 1: return StatusEnum.Fault;
                case 2: return StatusEnum.Zero;
                case 3: return StatusEnum.InMotion;
                case 4: return StatusEnum.Stable;
                case 5: return StatusEnum.UnderZero;
                case 6: return StatusEnum.OverWeight;
                case 7: return StatusEnum.NeedCalibration;
                case 8: return StatusEnum.NeedZeroing;
                default: return StatusEnum.Unknown;
            }
        }
    }
}