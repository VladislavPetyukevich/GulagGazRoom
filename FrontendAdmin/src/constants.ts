export const pathnames = {
  home: '/:redirect?',
  rooms: '/rooms',
  roomsCreate: '/rooms/create',
  roomsParticipants: '/rooms/participants/:id',
  room: '/rooms/:id',
  roomAnalyticsSummary: '/rooms/:id/analytics/summary',
  questions: '/questions',
  questionsCreate: '/questions/create',
  questionsEdit: '/questions/edit/:id',
  session: '/session',
  terms: '/terms',
};

export const enum IconNames {
  None = 'alert-circle',
  MicOn = 'mic',
  MicOff = 'mic-off',
  VideocamOn = 'videocam',
  VideocamOff = 'videocam-off',
  Settings = 'settings',
  RecognitionOn = 'volume-high',
  RecognitionOff = 'volume-mute',
  Chat = 'chatbubble-ellipses',
  Like = 'thumbs-up',
  Dislike = 'thumbs-down',
  Gas = 'thunderstorm',
  CodeEditor = 'code-slash',
  ThemeSwitchLight = 'sunny',
  ThemeSwitchDark = 'moon',
}

export const enum IconThemePostfix {
  Dark = '-sharp',
  Light = '-outline',
}

export const reactionIcon: Record<string, IconNames> = {
  Like: IconNames.Like,
  Dislike: IconNames.Dislike,
  Gas: IconNames.Gas,
  CodeEditor: IconNames.CodeEditor,
}

export const enum Captions {
  AppName = 'ГУЛАГ ГАЗ РУМ',
  AppDescription = 'Интерактивная платформа для проведения собеседований с возможностью составления детальных отчётов.',
  WelcomeMessage = 'Салют',
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
  QuestionUpdatedSuccessfully = 'Вопрос успешно обновлён',
  ShowClosedQuestions = 'Показывать закрытые вопросы',
  SelectActiveQuestion = 'Установить тему допроса',
  SendingActiveQuestion = 'Установка активного вопроса',
  LoadingRoom = 'Загрузка заседания',
  ErrorLoadingRoom = 'Ошибка загрузки заседания',
  LoadingRoomState = 'Загрузка состояния заседания',
  ErrorLoadingRoomState = 'Ошибка загрузки состояния заседания',
  RoomCreated = 'Заседание успешно создано',
  ErrorSendingActiveQuestion = 'Ошибка в установке активного вопроса',
  ReactionsLoadingError = 'Ошибка загрузки реакций',
  ErrorSendingReaction = 'Ошибка в отправке реакции',
  GetRoomEvent = 'Получение событий',
  ErrorGetRoomEvent = 'Ошибка в получении событий',
  ErrorSendingRoomEvent = 'Ошибка в отправке собтия',
  TermsOfUsage = 'Условия использования',
  Login = 'Представиться',
  WhoAreYou = 'Ты кто?',
  LikeTable = '👍',
  DislikeTable = '👎',
  Like = 'Хорош',
  Dislike = 'Плох',
  Gas = 'Газлайт',
  Question = 'Вопрос',
  ActiveQuestion = 'Текущий вопрос',
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
  StartReviewRoom = 'Закрыть комнату. 📔',
  CloseRoomModalTitle = 'Действительно хотите закрыть заседание?',
  StartReviewRoomModalTitle = 'Начать заполнение отчёта по заседанию?',
  CloseRoomLoading = 'Закрытие комнаты',
  Yes = 'Так точно ✔️',
  No = 'Никак нет! ❌',
  RoomStatusNew = 'Готовимся. Прогрев паяльников',
  RoomStatusActive = 'Идёт заседание',
  RoomStatusReview = 'На карандаше',
  RoomStatusClose = 'Санитарный час',
  Reviews = 'Доносы',
  AddReview = 'Написать донос',
  AddReviewPlaceholder = 'Писать донос сюда',
  Send = 'Отправить',
  WithLove = 'С любовью',
  TagsPlaceholder = 'Выбрать тэги',
  TagsLoading = 'Загрузка тэгов',
  NoTags = 'Тэги отсутствуют',
  SearchByTags = 'Поиск по тэгам',
  BuildHash = 'Хэш сборки',
  CreateRoom = 'Создание заседания',
  RoomName = 'Имя заседания',
  RoomTwitchChannel = 'Twitch канал',
  RoomQuestions = 'Поднимемые вопросы',
  RoomExperts = 'Начальники',
  RoomExaminees = 'Испытуемые',
  SearchByValue = 'Поиск по содержимому',
  Recognized = '🗣️',
  UserStreamError = 'Не удалось получить доступ к камере и микрофону',
  ChatWelcomeMessage = 'Добро пожаловать в ГУЛАГ',
  ChatMessagePlaceholder = 'Написать в чат',
  SendToChat = 'Чат',
  SearchByName = 'Поиск по имени',
  ParticipatingRooms = 'В которых я участвую',
  ClosedRooms = 'Закрытые',
  ToRooms = 'Перейти к заседаниям',
  Warning = 'ВНИМАНИЕ!',
  CallRecording = 'Разговор записыватеся',
  VoiceRecognitionNotSupported = 'Распознавание голоса не поддерживается вашим браузером',
  VoiceRecognition = 'Прослушка',
  ArchiveQuestion = 'Архивировать вопрос?',
  ArchiveQuestionLoading = 'Ахивирование вопроса...',
  NoQuestionsSelector = 'Нет доступных вопросов',
  Join = 'Присоединиться',
  JoiningRoom = 'Подключение к видеовстрече',
  JoinAs = 'Подключиться как',
  SetupDevices = 'Настроить камеру и микрофон',
  Camera = 'Камера',
  Microphone = 'Аудио',
  Settings = 'Настройки',
  Chat = 'Чат',
  Exit = 'Выйти',
  ChatTab = 'Чат',
  RecognitionTab = 'Транскрипция',
  Theme = 'Тема оформления',
  ThemeSystem = 'Системная',
  ThemeLight = 'Светлая',
  ThemeDark = 'Тёмная',
  Language = 'Язык',
  FontSize = 'Размер шрифта',
  You = 'Вы',
  Participants = 'Участники',
  NoRecords = 'Нет записей',
};

export const toastSuccessOptions = {
  icon: '👌',
};

export const reactionLocalization: Record<string, string> = {
  Like: Captions.Like,
  Dislike: Captions.Dislike,
  Gas: Captions.Gas,
}
