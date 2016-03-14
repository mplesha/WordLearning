using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataAccessLayer.Entities;
using DataAccessLayer.UnitOfWork;
using BusinessLayer.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BusinessLayer.Managers
{
    public class TutorialManager : ITutorialManager
    {
        private IUnitOfWork _unitOfWork;

        public TutorialManager()
        {
            _unitOfWork = new UnitOfWork();
        }

        public TutorialManager(IUnitOfWork unitOfWork)
        {
            if (unitOfWork == null)
                throw new ArgumentNullException("unitOfWork");

            _unitOfWork = unitOfWork;
        }

        public IList<TutorialModel> GetItemsFromWordSuite(int wordSuiteId)
        {
            var wordSuiteTranslations = new List<Translation>();
            wordSuiteTranslations = _unitOfWork.GetRepository<WordSuite>()
                .Get(wordSuite => wordSuite.WordSuiteId == wordSuiteId).First().Translations.ToList();

            var itemTranslationModel = new List<TutorialModel>();
            for (int i = 0; i < wordSuiteTranslations.Count; i++)
            {
                var itemModel = new TutorialModel();
                var itemTranslation = _unitOfWork.GetRepository<Translation>().GetByID(wordSuiteTranslations[i].TranslationId);

                itemModel.TitleWord = _unitOfWork.GetRepository<Item>().GetByID(itemTranslation.OriginalItemId.Value).Word;
                itemModel.PossibleAnswers = new List<string>();
                itemModel.Colors = new List<bool>();

                itemModel.PossibleAnswers.Add(_unitOfWork.GetRepository<Item>().GetByID(itemTranslation.TranslationItemId.Value).Word);
                itemModel.Colors.Add(true);

                var k = new Random((int)DateTime.Now.Ticks);
                int n;
                while (itemModel.PossibleAnswers.Count != 4)
                {
                    n = k.Next(wordSuiteTranslations.Count);
                    if ((wordSuiteTranslations[n].OriginalItemId.Value != itemTranslation.OriginalItemId.Value)
                        && (!itemModel.PossibleAnswers.Contains(_unitOfWork.GetRepository<Item>()
                        .GetByID(wordSuiteTranslations[n].TranslationItemId.Value).Word)))
                    {
                        itemModel.PossibleAnswers.Add(_unitOfWork.GetRepository<Item>()
                            .GetByID(wordSuiteTranslations[n].TranslationItemId.Value).Word);
                        itemModel.Colors.Add(false);
                    }
                }
                n = k.Next(4);

                SwapItem(itemModel, n);

                itemTranslationModel.Add(itemModel);
            }
            itemTranslationModel.Shuffle();
            return itemTranslationModel;
        }

        private void SwapItem(TutorialModel itemModel, int n)
        {
            string itemstring = itemModel.PossibleAnswers[n];
            itemModel.PossibleAnswers[n] = itemModel.PossibleAnswers[0];
            itemModel.PossibleAnswers[0] = itemstring;
            bool itembool = itemModel.Colors[n];
            itemModel.Colors[n] = itemModel.Colors[0];
            itemModel.Colors[0] = itembool;
        }


    }
}
