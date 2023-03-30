export const pathnames = {
  home: '/',
  rooms: '/rooms',
  roomsCreate: '/rooms/create',
  roomsParticipants: '/rooms/participants/:id',
  room: '/rooms/:id',
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
  SendingActiveQuestion = 'Установка активного вопроса',
  ErrorSendingActiveQuestion = 'Ошибка в установке активного вопроса',
  ReactionsLoadingError = 'Ошибка загрузки реакций',
  SendingReaction = 'Отправка реакции',
  ErrorSendingReaction = 'Ошибка в отправке реакции',
  SendingGasEvent = 'Отправка газа',
  ErrorSendingGasEvent = 'Ошибка в отправке газа',
  Reactions = 'Оценка собеседуемого',
  Gas = 'Газ',
  GasOn = 'Газ ВКЛ',
  GasOff = 'Газ ВЫКЛ',
  CopyRoomLink = 'Скопировать ссылку на заседание',
  TermsOfUsage = 'Условия использования',
  Login = 'Представиться',
  WhoAreYou = 'Ты кто?',
};
