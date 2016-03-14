using System;
using System.Collections.Generic;

using BusinessLayer.Models;
using DataAccessLayer.Entities;

namespace BusinessLayer.Interfaces
{
    public interface IQuizManager
    {
        List<ItemTranslationModel> GenerateQuiz(int currentUserId, int wordSuiteId);
        List<ItemTranslationModel> NotFirstQuizGeneration(int studentId, int wordSuiteId, List<LearningWords> userLearningWords);
        List<ItemTranslationModel> ItemTranslationsRand(List<ItemTranslationModel> itemTranslations);
        List<LearningWords> ItemsLearning(int currentUserId, List<ItemTranslationModel> itemTranslations);

        QuizModel QuizAnswers(int currentUserId, List<ItemTranslationModel> itemTranslations, List<string> userTranslations);
        DateTime CheckQuizTimeout(LearningWords userLearningWord);
    }
}
