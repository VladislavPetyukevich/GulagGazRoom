export const pathnames = {
  home: '/',
  rooms: '/rooms',
  roomsCreate: '/rooms/create',
  roomsParticipants: '/rooms/participants/:id',
  room: '/rooms/:id',
  roomAnalyticsSummary: '/rooms/:id/analytics/summary',
  questions: '/questions',
  questionsCreate: '/questions/create',
  session: '/session',
  terms: '/terms',
};

export const enum Captions {
  AppName = 'ГУЛАГ ГАЗ РУМ',
  WelcomeMessage = 'Салют',
  HomePageName = 'Проходная',
  RoomsPageName = 'Заседания',
  QuestionsPageName = 'Вопросы',
  UnauthorizedMessage = 'Не авторизован',
  Page = 'Страница',
  LogOut = 'Закрой за мной дверь, я ухожу',
  EditParticipants = 'Редактировать звания',
  Viewer = 'Гражданский',
  Viewers = 'Обычные граждане',
  Expert = 'Начальник',
  Examinee = 'Испытуемый',
  Save = 'Сохранить',
  CreateQuestion = 'Создать вопрос',
  QuestionText = 'Текст вопроса',
  Create = 'Создать',
  Error = 'Ошибка',
  QuestionCreatedSuccessfully = 'Вопрос успешно создан',
  Room = 'Заседание',
  SetActiveQuestion = 'Сделать активным вопросом',
  ShowClosedQuestions = 'Показывать закрытые вопросы',
  SelectActiveQuestion = 'Установить тему допроса',
  SendingActiveQuestion = 'Установка активного вопроса',
  LoadingRoom = 'Загрузка заседания',
  ErrorLoadingRoom = 'Ошибка загрузки заседания',
  RoomCreated = 'Заседание успешно создано',
  ErrorSendingActiveQuestion = 'Ошибка в установке активного вопроса',
  ReactionsLoadingError = 'Ошибка загрузки реакций',
  SendingReaction = 'Отправка реакции',
  ErrorSendingReaction = 'Ошибка в отправке реакции',
  SendingGasEvent = 'Отправка газа',
  ErrorSendingGasEvent = 'Ошибка в отправке газа',
  SendingCodeEditorEvent = 'Открытие редактора',
  ErrorSendingCodeEditorEvent = 'Ошибка в открытии редактора',
  LikeReactions = 'Хорош',
  DislikeReactions = 'Плох',
  Gas = 'Газ',
  CodeEditor = 'Редактор кода',
  On = 'ВКЛ',
  Off = 'ВЫКЛ',
  CopyRoomLink = 'Скопировать ссылку на заседание',
  TermsOfUsage = 'Условия использования',
  Login = 'Представиться',
  WhoAreYou = 'Ты кто?',
  LikeTable = '👍',
  DislikeTable = '👎',
  Question = 'Вопрос',
  ActiveQuestion = 'Текущий вопрос',
  Summary = 'Отчёт',
  QuestionsSummary = 'Отчётики на вопросики',
  FailedToCalculateMark = 'Ошибка при подсчёте оценки',
  RoomAnayticsSummary = 'Официальный отчет заседания',
  MarkNotCalculated = 'Оценка не рассчитана',
  MarkWithPlus = 'с плюсом',
  MarkWithMinus = 'с минусом',
  MarkAveragePrefix = 'Чисто',
  MarkAverage = 'без плюса и минуса, без крестика и без нолика',
  MarkPostfixCool = 'Ну ты и даёшь, братишка.',
  MarkPostfixAverage = 'То густо, то пусто. Продолжай, брат.',
  MarkPostfixBad = 'Надо тренироваться, брат.',
  MarkSmmary = 'Чтож сказать',
  CloseRoom = 'Закрыть заседание ❌',
  WaitingRoom = 'Ожидание начала заседания.',
  StartReviewRoom = 'Закрыть заседание. Начинаем писать отчёт. 📔',
  CloseRoomModalTitle = 'Действительно хотите закрыть заседание?',
  StartReviewRoomModalTitle = 'Начать заполнение отчёта по заседанию?',
  CloseRoomLoading = 'Закрытие комнаты',
  Yes = 'Так точно ✔️',
  No = 'Никак нет! ❌',
  Status = 'Статус',
  RoomStatusNew = 'Готовимся. Прогрев паяльников',
  RoomStatusActive = 'Идёт заседание',
  RoomStatusReview = 'На карандаше',
  RoomStatusClose = 'Санитарный час',
  Reviews = 'Доносы',
  AddReview = 'Написать донос',
  AddReviewPlaceholder = 'Писать донос сюда',
  Send = 'Отправить',
  WithLove = 'С любовью',
};

export const toastSuccessOptions = {
  icon: '👌',
};
