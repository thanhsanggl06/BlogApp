import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { Category } from '../models/add-category-request.model';
import { CategoryService } from '../services/category.service';
import { UpdateCategoryRequest } from '../models/update-category-request.model';

@Component({
  selector: 'app-edit-category',
  templateUrl: './edit-category.component.html',
  styleUrls: ['./edit-category.component.css'],
})
export class EditCategoryComponent implements OnInit, OnDestroy {
  id: string | null = null;
  paramsSubscription?: Subscription;
  editCategorySubscription?: Subscription;
  deleteCategorySubscription?: Subscription;
  categoriesSubscription?: Subscription;
  category?: Category;

  constructor(
    private route: ActivatedRoute,
    private categoriesService: CategoryService,
    private router : Router
  ) {}

  ngOnInit(): void {
    this.paramsSubscription = this.route.paramMap.subscribe({
      next: (params) => {
        this.id = params.get('id');

        if (this.id) {
          this.categoriesSubscription = this.categoriesService
            .getCategoryById(this.id)
            .subscribe({
              next: (response) => {
                this.category = response;
              },
            });
        }
      },
    });
  }

  ngOnDestroy(): void {
    this.paramsSubscription?.unsubscribe();
    this.categoriesSubscription?.unsubscribe();
    this.editCategorySubscription?.unsubscribe();
    this.deleteCategorySubscription?.unsubscribe();
  }

  onFormSubmit(): void{
    const updateCategoryRequest : UpdateCategoryRequest = {
      name : this.category?.name ?? '',
      urlHandle : this.category?.urlHandle ?? ''
    }
    if(this.id){
       this.editCategorySubscription = this.categoriesService.updateCategory(this.id, updateCategoryRequest).subscribe({
        next : (response) =>{
          this.router.navigateByUrl('/admin/categories');
        }
      })
    }
  }

  onDelete() : void{
    var result = confirm('Bạn có chắc chắn muốn xóa?')
    if(this.id && result){
      this.deleteCategorySubscription = this.categoriesService.deleteCategory(this.id)
      .subscribe({
        next : (response) =>{
          this.router.navigateByUrl('/admin/categories');
        }
      })
    }
  }
}
