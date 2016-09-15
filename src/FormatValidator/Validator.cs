﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FormatValidator.Configuration;
using FormatValidator.Input;
using FormatValidator.Validators;

namespace FormatValidator
{
    public class Validator
    {
        private RowValidator _rowValidator;

        public Validator()
        {
            _rowValidator = new RowValidator(',');
        }

        public static Validator FromJson(string json)
        {
            ValidatorConfiguration configuration = new JsonReader().Read(json);
            ConvertedValidators converted = new Converter().Convert(configuration);

            Validator validator = new Validator();
            validator.TransferConvertedColumns(converted);

            return validator;
        }
        
        public List<ValidationError> Validate(ISourceReader reader)
        {
            List<ValidationError> errors = new List<ValidationError>();

            foreach(string line in reader.ReadLines())
            {
                _rowValidator.IsValid(line);
                errors.AddRange(_rowValidator.GetErrors());
                _rowValidator.ClearErrors();
            }

            return errors;
        }

        public List<ValidatorGroup> GetColumnValidators()
        {
            return _rowValidator.GetColumnValidators();
        }

        private void TransferConvertedColumns(ConvertedValidators converted)
        {
            _rowValidator = new RowValidator(',');

            foreach (KeyValuePair<int, List<IValidator>> column in converted.Columns)
            {
                foreach (IValidator columnValidator in column.Value)
                {
                    _rowValidator.AddColumnValidator(column.Key, columnValidator);
                }
            }
        }
    }
}
