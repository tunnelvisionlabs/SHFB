﻿//===============================================================================================================
// System  : Sandcastle Help File Builder Visual Studio Package
// Author  : Sam Harwell  (sam@tunnelvisionlabs.com)
// Note    : Copyright 2014, Sam Harwell, All rights reserved
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//===============================================================================================================

namespace SandcastleBuilder.Package.IntelliSense
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.VisualStudio.Language.Intellisense;

    /// <summary>
    /// This class extends <see cref="CompletionSet"/> to allow the results contained in
    /// two independent <see cref="CompletionSet"/> instances to be presented in a single
    /// IntelliSense suggestions list.
    /// </summary>
    public class AugmentedCompletionSet : CompletionSet, IDisposable
    {
        /// <summary>
        /// This is the original completion set.
        /// </summary>
        private readonly CompletionSet _source;

        /// <summary>
        /// This completion set contains the injected "additional" items.
        /// </summary>
        private readonly CompletionSet _secondSource;

        /// <summary>
        /// This is the backing field for the <see cref="Completions"/> property.
        /// </summary>
        /// <remarks>
        /// Visual Studio relies on this object extending <see cref="ObservableCollection{T}"/>,
        /// despite the fact that the <see cref="Completions"/> property only returns
        /// <see cref="IList{T}"/>.
        /// </remarks>
        private readonly BulkObservableCollection<Completion> _completions = new BulkObservableCollection<Completion>();

        /// <summary>
        /// This is the backing field for the <see cref="CompletionBuilders"/> property.
        /// </summary>
        /// <remarks>
        /// Visual Studio relies on this object extending <see cref="ObservableCollection{T}"/>,
        /// despite the fact that the <see cref="CompletionBuilders"/> property only returns
        /// <see cref="IList{T}"/>.
        /// </remarks>
        private readonly BulkObservableCollection<Completion> _completionBuilders = new BulkObservableCollection<Completion>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AugmentedCompletionSet"/> class from
        /// the specified <paramref name="source"/> <see cref="CompletionSet"/> and a second
        /// completion set used to augment the source set.
        /// </summary>
        /// <param name="source">The source completion set.</param>
        /// <param name="secondSource">The second completion set used to augment <paramref name="source"/></param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="source"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="secondSource"/> is <see langword="null"/>.</para>
        /// </exception>
        public AugmentedCompletionSet(CompletionSet source, CompletionSet secondSource)
            : base(source.Moniker, source.DisplayName, source.ApplicableTo, source.Completions, source.CompletionBuilders)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (secondSource == null)
                throw new ArgumentNullException("secondSource");

            _source = source;
            _secondSource = secondSource;
            UpdateCompletionLists();
        }

        /// <inheritdoc/>
        public override IList<Completion> Completions
        {
            get
            {
                return _completions;
            }
        }

        /// <inheritdoc/>
        public override IList<Completion> CompletionBuilders
        {
            get
            {
                return _completionBuilders;
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method calls <see cref="Filter"/> on each of the underlying completion sets,
        /// and then updates the collections returned by <see cref="Completions"/> and
        /// <see cref="CompletionBuilders"/> to properly reflect the complete set of items
        /// remaining in the underlying collections.
        /// </remarks>
        public override void Filter()
        {
            _source.Filter();
            _secondSource.Filter();
            UpdateCompletionLists();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method calls <see cref="Recalculate"/> on each of the underlying completion
        /// sets, and then updates the following items to reflect the current state of the
        /// underlying collections.
        ///
        /// <list type="bullet">
        /// <item>The collections returned by <see cref="Completions"/> and
        /// <see cref="CompletionBuilders"/>.</item>
        /// <item>The <see cref="CompletionSet.ApplicableTo"/>,
        /// <see cref="CompletionSet.Moniker"/>, and <see cref="CompletionSet.DisplayName"/>
        /// properties are updated to reflect the changes (if any) which were made to the
        /// original <see cref="CompletionSet"/> during the recalculation.</item>
        /// </list>
        /// </remarks>
        public override void Recalculate()
        {
            _source.Recalculate();
            _secondSource.Recalculate();

            this.ApplicableTo = _source.ApplicableTo;
            this.Moniker = _source.Moniker;
            this.DisplayName = _source.DisplayName;
            UpdateCompletionLists();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This method first checks the <see cref="CompletionSet.SelectionStatus"/> property to
        /// determine if a <see cref="Completion"/> has been manually selected. This condition is
        /// identified by <em>all</em> of the following conditions being true.
        ///
        /// <list type="bullet">
        /// <item><see cref="CompletionSelectionStatus.Completion"/> is not <see langword="null"/>.</item>
        /// <item><see cref="CompletionSelectionStatus.IsSelected"/> is <see langword="true"/>.</item>
        /// <item><see cref="CompletionSelectionStatus.IsUnique"/> is <see langword="true"/>.</item>
        /// <item><see cref="CompletionSet.SelectionStatus"/> is not equal to the
        /// <see cref="CompletionSet.SelectionStatus"/> value for either of the underlying completion
        /// sets; i.e. it was explicitly set for the augmented completion set.</item>
        /// </list>
        ///
        /// <para>Next, this method ensures that the last typed character is actually a letter.
        /// This test prevents <see cref="SelectBestMatch"/> from changing the current selection
        /// when the user types a non-letter character to commit the selection, such as <c>&lt;</c>,
        /// <c>/</c>, or <c>"</c>. When a non-letter character is typed, the augmented set
        /// assumes that the correct selection was made during a previous call to
        /// <see cref="SelectBestMatch"/>.</para>
        ///
        /// <para>This method calls <see cref="SelectBestMatch"/> on each of the underlying completion
        /// sets. If these calls result in a <em>single</em> unique selection, that selection is
        /// assigned to the <see cref="CompletionSet.SelectionStatus"/> property. Otherwise, the base
        /// implementation is called to determine the best selection from the augmented completion
        /// set.</para>
        ///
        /// <note type="note">A future implementation might provide a special interface that allows
        /// selecting a best match across multiple source <see cref="CompletionSet"/> instances, but
        /// for now we must rely on the default algorithm operating on the merged collections.</note>
        /// </remarks>
        public override void SelectBestMatch()
        {
            // don't overwrite manually selected results
            if (SelectionStatus != null && SelectionStatus.Completion != null && SelectionStatus.IsSelected && SelectionStatus.IsUnique)
            {
                if (SelectionStatus != _source.SelectionStatus && SelectionStatus != _secondSource.SelectionStatus)
                    return;
            }

            // if the typed character wasn't a letter, ignore this request (fixes handling of /, >, and other characters triggering a completion)
            string completionText = ApplicableTo.GetText(ApplicableTo.TextBuffer.CurrentSnapshot);
            if (completionText.Length > 0 && !char.IsLetter(completionText[completionText.Length - 1]))
                return;

            _source.SelectBestMatch();
            _secondSource.SelectBestMatch();

            bool firstUnique = _source.SelectionStatus != null && _source.SelectionStatus.IsUnique && !_secondSource.SelectionStatus.IsSelected;
            bool secondUnique = _secondSource.SelectionStatus != null && _secondSource.SelectionStatus.IsUnique && !_source.SelectionStatus.IsSelected;
            if (firstUnique && !secondUnique && _source.SelectionStatus.IsSelected)
                this.SelectionStatus = _source.SelectionStatus;
            else if (secondUnique && !firstUnique && _secondSource.SelectionStatus.IsSelected)
                this.SelectionStatus = _secondSource.SelectionStatus;
            else
                base.SelectBestMatch();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Immediately releases resources owned by this object.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true"/> if this method was called by <see cref="Dispose()"/>;
        /// otherwise, <see langword="false"/> if this method was called by a finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                IDisposable sourceDisposable = _source as IDisposable;
                if (sourceDisposable != null)
                    sourceDisposable.Dispose();

                IDisposable secondSourceDisposable = _secondSource as IDisposable;
                if (secondSourceDisposable != null)
                    secondSourceDisposable.Dispose();
            }
        }

        /// <summary>
        /// Update the backing collections for <see cref="Completions"/> and <see cref="CompletionBuilders"/>
        /// to reflect the current collections provided by the underlying completion sets.
        /// </summary>
        private void UpdateCompletionLists()
        {
            UpdateCompletionList(_completions, _source.Completions, _secondSource.Completions);
            UpdateCompletionList(_completionBuilders, _source.CompletionBuilders, _secondSource.CompletionBuilders);
        }

        /// <summary>
        /// Perform a bulk update of a collection of <see cref="Completion"/> instances to match the
        /// values contained in a pair of source collections.
        /// </summary>
        /// <remarks>
        /// The <see cref="Completion"/> instances in <paramref name="sourceCompletions"/> and
        /// <paramref name="secondSourceCompletions"/> are merged and sorted using
        /// <see cref="CompletionDisplayTextComparer.Default"/>.
        ///
        /// <list type="bullet">
        /// <item>Any items present in <paramref name="result"/> but not present in the merged
        /// collection are removed from <paramref name="result"/>.</item>
        /// <item>Any items not already present in <paramref name="result"/> are inserted at
        /// the correct location.</item>
        /// </list>
        ///
        /// <note type="caller">
        /// The <paramref name="result"/> collection is assumed to already be sorted according
        /// to <see cref="CompletionDisplayTextComparer.Default"/> before this method is called.
        /// </note>
        /// </remarks>
        /// <param name="result">The collection holding the merged result.</param>
        /// <param name="sourceCompletions">The collection of <see cref="Completion"/> instances from the first source.</param>
        /// <param name="secondSourceCompletions">The collection of <see cref="Completion"/> instances from the second source.</param>
        private static void UpdateCompletionList(BulkObservableCollection<Completion> result, IEnumerable<Completion> sourceCompletions, IEnumerable<Completion> secondSourceCompletions)
        {
            result.BeginBulkOperation();
            try
            {
                // SortedSet<Completion> provides for immediate sorting, and also allows Contains()
                // to be used below instead of a manual call to CompletionDisplayTextComparer.Compare().
                SortedSet<Completion> filteredCompletions = new SortedSet<Completion>(CompletionDisplayTextComparer.Default);
                filteredCompletions.UnionWith(sourceCompletions);
                filteredCompletions.UnionWith(secondSourceCompletions);

                int j = 0;
                foreach (Completion completion in filteredCompletions)
                {
                    while (j < result.Count && !filteredCompletions.Contains(result[j]))
                        result.RemoveAt(j);

                    if (j == result.Count || completion != result[j])
                    {
                        result.Insert(j, completion);
                        j++;
                    }
                }
            }
            finally
            {
                result.EndBulkOperation();
            }
        }
    }
}
