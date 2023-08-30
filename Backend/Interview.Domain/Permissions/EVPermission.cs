using System.ComponentModel;

namespace Interview.Domain.Permissions;

public enum EVPermission
{
    Unknown,

    [Description("Получение страницы вопросов")]
    QuestionFindPage,

    [Description("Получение страницы архивных вопросов")]
    QuestionFindPageArchive,

    [Description("Создание нового вопроса")]
    QuestionCreate,

    [Description("Обновление вопроса")]
    QuestionUpdate,

    [Description("Поиск вопроса по идентификатору")]
    QuestionFindById,

    [Description("Перманентное удаление вопроса")]
    QuestionDeletePermanently,

    [Description("Архивация вопроса")]
    QuestionArchive,

    [Description("Разархивация вопроса")]
    QuestionUnarchive,

    [Description("Получение страницы реакций")]
    ReactionFindPage,

    [Description("Получение участника комнаты")]
    RoomParticipantFindByRoomIdAndUserId,

    [Description("Изменение статуса участника в комнате")]
    RoomParticipantChangeStatus,

    [Description("Добавление нового участника в комнату")]
    RoomParticipantCreate,

    [Description("Добавление реакции на активный вопрос в комнате")]
    RoomQuestionReactionCreate,

    [Description("Установка активного вопроса в комнате")]
    RoomQuestionChangeActiveQuestion,

    [Description("Добавление существующего вопроса в комнату")]
    RoomQuestionCreate,

    [Description("Получение списка идентификаторов вопросов в комнате по идентификатору комнаты и статусу вопроса")]
    RoomQuestionFindGuids,

    [Description("Получение списка отзывов")]
    RoomReviewFindPage,

    [Description("Создание нового отзыва")]
    RoomReviewCreate,

    [Description("Обновление отзыва")]
    RoomReviewUpdate,

    [Description("Создание новой комнаты")]
    RoomCreate,

    [Description("Получение страницы комнат")]
    RoomFindPage,

    [Description("Получение комнаты по идентификатору")]
    RoomFindById,

    [Description("Обновление комнаты")]
    RoomUpdate,

    [Description("Добавление нового участника в комнату")]
    RoomAddParticipant,

    [Description("Отправка события в комнату")]
    RoomSendEventRequest,

    [Description("Закрытие комнаты")]
    RoomClose,

    [Description("Перевод комнаты в стадию - отзывы")]
    RoomStartReview,

    [Description("Получение статуса комнаты")]
    RoomGetState,

    [Description("Получение полной аналитики по результатам собеседования")]
    RoomGetAnalytics,

    [Description("Получение краткой аналитики по результатам собеседования")]
    RoomGetAnalyticsSummary,

    [Description("Получение страницы пользователей")]
    UserFindPage,

    [Description("Получение пользователя по логину")]
    UserFindByNickname,

    [Description("Получение пользователя по идентификатору")]
    UserFindById,

    [Description("Обновление данных пользователя по twitch идентификатору")]
    UserUpsertByTwitchIdentity,

    [Description("Получение страницы пользователей по роли")]
    UserFindByRole,

    [Description("Получение списка разрешений пользователя по идентификатору")]
    UserGetPermissions,

    [Description("Предоставление пользователю разрешения")]
    UserChangePermission,
}
