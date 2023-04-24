const questions = Array.from({ length: 4 }, (_, k) => `question${k + 1}`);

const authCookieName = '_communist';

describe('Interview flow', () => {
  beforeEach(() => {
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
});
