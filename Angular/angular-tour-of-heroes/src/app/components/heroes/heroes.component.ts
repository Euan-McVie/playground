import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Hero } from 'src/app/models/hero';
import { HeroService } from 'src/app/services/hero.service';
import { MessageService } from 'src/app/services/message.service';
import { Location } from '@angular/common';

@Component({
  selector: 'app-heroes',
  templateUrl: './heroes.component.html',
  styleUrls: ['./heroes.component.scss']
})
export class HeroesComponent {
  heroes: Hero[] = [];
  selectedHero?: Hero;
  newHero: boolean = false;

  constructor(
    private heroService: HeroService,
    private messageService: MessageService,
    private route: ActivatedRoute,
    private location: Location
  ) { }

  ngOnInit(): void {
    this.getHeroes();
    this.setIncomingHero();
  }

  private setIncomingHero(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id != null)
      this.heroService.getHero(Number(id))
        .subscribe(hero => this.onSelect(hero));
  }

  private getHeroes(): void {
    this.heroService.heroes$
      .subscribe(heroes => this.heroes = heroes);
  }

  addHero(): void {
    this.newHero = true;
  }

  removeHero(hero: Hero): void {
    this.heroService.deleteHero(hero.id).subscribe();
  }

  clearSelected() {
    this.selectedHero = undefined;
    this.newHero = false;
    this.location.replaceState('/heroes')
  }

  onSelect(hero: Hero): void {
    if (this.selectedHero?.id === hero.id) {
      this.clearSelected();
    }
    else {
      this.selectedHero = hero;
      this.location.replaceState(`/heroes/${hero.id}`)
    }
    this.messageService.add(`HeroesComponent: Selected hero id=${hero.id}`);
  }
}
