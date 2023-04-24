const questions = Array.from({ length: 4 }, (_, k) => `question${k + 1}`);

const questionsForRoom = [questions[0], questions[2], questions[3]];

const authCookieName = '_communist';
const username = Cypress.env('username');
const roomName = 'Room 1';
const interviewFrontendUrl = 'http://localhost:8080';

describe('Interview flow', () => {
  before(() => {
    cy.setCookie(authCookieName, Cypress.env(authCookieName));
    cy.visit('http://localhost:3000');
  });

  it('Create questions', () => {
    cy.get('[data-cy="nav-/questions"]').click();
    cy.get('[data-cy="header-link-/questions/create"]').click();

    cy.wrap(questions).each(question => {
      cy.get('#qestionText').clear();
      cy.get('#qestionText').type(question);
      cy.get('[data-cy="qestion-submit"]').click();
      cy.get('[data-cy="label-question-created-successfully"]').should('be.visible');
    });

    cy.get('[data-cy="header-link-/questions"]').click();
    cy.wrap(questions).each(question => {
      cy.get('[data-cy="question-item"]').contains(new RegExp(`^${question}$`)).should('be.visible');
    });
  });

  it('Create room', () => {
    cy.get('[data-cy="nav-/rooms"]').click();
    cy.get('[data-cy="header-link-/rooms/create"]').click();

    cy.get('input[name="roomName"]').type(roomName);
    cy.get('input[name="roomTwitchChannel"]').type('masonyan777');
    cy.wrap(questionsForRoom).each(question => {
      cy.get(`[data-cy="questions-selector-${question}"]`).click();
    });
    cy.get(`[data-cy="users-selector-${username}"]`).click();
    cy.get('[data-cy="room-create-submit"]').click();
    cy.get('[data-cy="room-created-successfully"]').should('be.visible');
    cy.get('[data-cy="header-link-/rooms"]').click();
    cy.get(`[data-cy="room-link-${roomName}"]`).should('be.visible');
  });

  describe('Room interactions', () => {
    it('Open room', () => {
      cy.get('[data-cy="nav-/rooms"]').click();
      cy.get(`[data-cy="room-link-${roomName}"]`).click();
      cy.get('h2').contains(roomName).should('be.visible');
    });

    it('Change active question', () => {
      cy.get('[data-cy="active-question-selector"]').click();
      cy.get(`[data-cy="active-question-selector-${questionsForRoom[2]}"]`).click();
      cy.get('[data-cy="error-sending-active-question"]').should('not.exist');
      cy.get('[data-cy="sending-active-question"]').should('not.exist');
      cy.get('[data-cy="active-question-selector"]').click();
      cy.get(`[data-cy="active-question-selector-${questionsForRoom[0]}"]`).click();
      cy.get('[data-cy="error-sending-active-question"]').should('not.exist');
      cy.get('[data-cy="sending-active-question"]').should('not.exist');
    });

    it('Change to closed question', () => {
      cy.get('[data-cy="checkbox-closed-questions"]').click();
      cy.get('[data-cy="active-question-selector"]').click();
      cy.get(`[data-cy="active-question-selector-${questionsForRoom[2]}"]`).should('be.visible');
      cy.get(`[data-cy="active-question-selector-${questionsForRoom[2]}"]`).click();
      cy.get('[data-cy="sending-active-question"]').should('not.exist');
      cy.get('[data-cy="error-sending-active-question"]').should('not.exist');
    });
  });
});
