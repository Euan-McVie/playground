import { TestBed } from '@angular/core/testing';

import { HeroService } from './hero.service';
import { TestScheduler } from 'rxjs/testing';
import { InitialHeroes } from './in-memory-data.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

describe('HeroService', () => {
  const heroesUrl = 'api/heroes';
  let service: HeroService;
  let httpTestingController: HttpTestingController;
  let testScheduler: TestScheduler;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
    });
    httpTestingController = TestBed.inject(HttpTestingController);
    testScheduler = new TestScheduler((actual, expected) => expect(actual).toEqual(expected));
    service = TestBed.inject(HeroService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should return expected hero list', () => {
    testScheduler.run(({ expectObservable }) => {
      // Arrange
      const expectedMarble = '(a)';
      const expectedHeroes = { a: InitialHeroes };

      // Act
      service.heroes$.subscribe();

      // Assert
      const req = httpTestingController.expectOne(heroesUrl);
      req.flush(InitialHeroes);
      expectObservable(service.heroes$).toBe(expectedMarble, expectedHeroes);
    });
  });
});
