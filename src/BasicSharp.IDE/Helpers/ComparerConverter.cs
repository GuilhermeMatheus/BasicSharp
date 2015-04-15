using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;


namespace BasicSharp.IDE.Helpers
{
    public enum ComparerOperator { Equals, NotEquals }
    public enum CommomValues { Bool, Visibility, UseTargetType }

    public class ComparerConverter : IValueConverter
    {
        public IValueConverter Converter { get; set; }

        public object TrueValue { get; set; }
        public object FalseValue { get; set; }

        public object TargetValue { get; set; }
        public object UnTargetValue { get; set; }
        public bool UseParameterAsTarget { get; set; }

        CommomValues commomValues;
        public CommomValues CommomValues
        {
            get { return commomValues; }
            set
            {
                commomValues = value;
                prepareValues();
            }
        }

        public ComparerOperator Operator { get; set; }

        public ComparerConverter()
        {
            prepareValues();
        }

        #region IValueConverter Implementation
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (CommomValues == CommomValues.UseTargetType)
                prepareTargetValues(targetType);

            if (parameter != null)
                parameter.ToString();

            if (Converter != null)
                value = Converter.Convert(value, targetType, parameter, culture);

            if (UseParameterAsTarget)
                return returnHelper(parameter, value);
            else
                return returnHelper(value, TargetValue);
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (object.Equals(value, TrueValue))
                return TargetValue;
            else if (object.Equals(value, FalseValue))
                return UnTargetValue;
            else
                throw new NotImplementedException();
        }
        #endregion

        private void prepareTargetValues(Type targetType)
        {
            if (targetType == typeof(Visibility))
            {
                TrueValue = Visibility.Visible;
                FalseValue = Visibility.Collapsed;
            }
            else if (targetType == typeof(Boolean))
            {
                TrueValue = true;
                FalseValue = false;
            }
        }
        private void prepareValues()
        {
            switch (CommomValues)
            {
                case CommomValues.Visibility:
                    if (TrueValue is bool && FalseValue is bool)
                    {
                        TrueValue = Visibility.Visible;
                        FalseValue = Visibility.Collapsed;
                    }
                    break;
                case CommomValues.Bool:
                    TrueValue = TrueValue ?? true;
                    FalseValue = FalseValue ?? false;
                    break;
                default:
                    break;
            }
        }
        private object returnHelper(object objA, object objB)
        {
            if (compare(objA, objB))
                return TrueValue;
            else
                return FalseValue;
        }


        private bool compare(object objA, object objB)
        {
            switch (Operator)
            {
                case ComparerOperator.Equals:
                    if (objA == null && objB == null)
                        return true;
                    else if (objA == null || objB == null)
                        return false;
                    else 
                        return objA.Equals(objB);
                case ComparerOperator.NotEquals:
                    if (objA == null && objB == null)
                        return false;
                    else if (objA == null || objB == null)
                        return true;
                    else 
                    return !objA.Equals(objB);
            }
            throw new NotImplementedException("Tipo de comparação não esperada");
        }
    }
}
