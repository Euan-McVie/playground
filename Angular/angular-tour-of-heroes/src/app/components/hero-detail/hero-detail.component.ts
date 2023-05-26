import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { Hero } from 'src/app/models/hero';
import { HeroService } from 'src/app/services/hero.service';

class HeroDetail {
  id?: number = undefined;
  name: string = '';
}

@Component({
  selector: 'app-hero-detail',
  templateUrl: './hero-detail.component.html',
  styleUrls: ['./hero-detail.component.scss']
})
export class HeroDetailComponent implements OnChanges {
  @Input() heroId?: number;
  @Input() newHero: boolean = false;
  @Output() heroSaved = new EventEmitter();
  hero?: HeroDetail;

  constructor(private heroService: HeroService) { }

  ngOnChanges(changes: SimpleChanges): void {
    const currentHero = changes['heroId']?.currentValue;
    const prevHero = changes['heroId']?.previousValue;
    if (this.newHero)
      this.hero = new HeroDetail();
    else if (currentHero == null)
      this.hero = undefined;
    else if (currentHero !== prevHero)
      this.fetchHero(currentHero);
  }

  private fetchHero(id: number): void {
    if (id != null)
      this.heroService.getHero(Number(id))
        .subscribe(hero => this.hero = hero);
  }

  onSubmit(): void {
    if (this.hero) {
      if (this.hero.id != undefined)
        this.heroService.updateHero(this.hero as Hero)
          .subscribe(() => this.heroSaved.emit());
      else
        this.heroService.addHero(this.hero as Hero)
          .subscribe(() => this.heroSaved.emit());
    }
  }
}
