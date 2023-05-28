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
  Reactions = 'Оценка члена',
  Gas = 'Газ',
  GasOn = 'ВКЛ',
  GasOff = 'ВЫКЛ',
  CopyRoomLink = 'Скопировать ссылку на заседание',
  TermsOfUsage = 'Условия использования',
  Login = 'Представиться',
  WhoAreYou = 'Ты кто?',
  LikeTable = '👍',
  DislikeTable = '👎',
  Question = 'Вопрос',
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
  CloseRoomModalTitle = 'Действительно хотите закрыть заседание?',
  CloseRoomLoading = 'Закрытие комнаты',
  Yes = 'Так точно ✔️',
  No = 'Никак нет! ❌',
};

export const toastSuccessOptions = {
  icon: '👌',
};
