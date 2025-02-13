using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace L_AI.CodeAnalysis
{
    public abstract class BaseAnalyzer<T> where T : class
    {
        public virtual string LanguageCode { get; }

        private static T _instance;
        public static T Instance => _instance ?? (_instance = Activator.CreateInstance<T>());

        public abstract Task<string> GetSourceCodeFromClassAsync(string className);
        public abstract Task<IEnumerable<ISymbol>> GetUniqueReferencesFromClassAsync(string className);
        public abstract Task<IEnumerable<string>> GetAllReferenesAsTextAsync(string className);
        public abstract Task<IEnumerable<string>> GetAllReferenesAsTextFromPathAsync(string path);


        public abstract Task Test();
    }
}
