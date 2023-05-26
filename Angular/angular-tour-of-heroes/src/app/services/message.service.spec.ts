import { TestBed } from '@angular/core/testing';

import { MessageService } from './message.service';

describe('MessageService', () => {
  let service: MessageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MessageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should append a message when add called', () => {
    // Act
    service.add("TestAdd");

    // Assert
    expect(service.messages).toEqual(["TestAdd"]);
  });

  it('should empty messages when clear called', () => {
    // Arrange
    service.messages.push("TestClear")

    // Act
    service.clear();

    // Assert
    expect(service.messages).toEqual([]);
  });
});
