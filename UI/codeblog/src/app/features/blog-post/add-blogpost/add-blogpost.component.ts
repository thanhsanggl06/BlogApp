import { Component, OnDestroy, OnInit } from '@angular/core';
import { AddBlogPost } from '../models/add-blog-post.model';
import { BlogPostService } from '../services/blog-post.service';
import { Observable, Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { CategoryService } from '../../category/services/category.service';
import { Category } from '../../category/models/add-category-request.model';
import { ImageService } from 'src/app/shared/components/image-selector/image.service';

@Component({
  selector: 'app-add-blogpost',
  templateUrl: './add-blogpost.component.html',
  styleUrls: ['./add-blogpost.component.css'],
})
export class AddBlogpostComponent implements OnInit, OnDestroy {
  model: AddBlogPost;
  creatBlogPostSubscription?: Subscription;
  categories$?: Observable<Category[]>;
  isImageSeletorVisible: boolean = false;
  imageSelectSubscription?: Subscription;

  constructor(
    private blogPostService: BlogPostService,
    private router: Router,
    private categoryService: CategoryService,
    private imageService: ImageService
  ) {
    this.model = {
      title: '',
      shortDescription: '',
      content: '',
      author: '',
      featuredImageUrl: '',
      urlHandle: '',
      isVisible: true,
      publishedDate: new Date(),
      categories: [],
    };
  }

  ngOnDestroy(): void {
    this.creatBlogPostSubscription?.unsubscribe();
    this.imageSelectSubscription?.unsubscribe();
  }

  ngOnInit(): void {
    this.categories$ = this.categoryService.getAllCategories();

    this.imageSelectSubscription = this.imageService.onSelectImage().subscribe({
      next: (selectedImage) => {
        this.model.featuredImageUrl = selectedImage.url;
        this.closeImageSelector();
      },
    });
  }

  onFormSubmit(): void {
    this.creatBlogPostSubscription = this.blogPostService
      .createBlogPost(this.model)
      .subscribe({
        next: (response) => {
          this.router.navigateByUrl('/admin/blogposts');
        },
      });
    // console.log(this.model);
  }

  openImageSeletor(): void {
    this.isImageSeletorVisible = true;
  }

  closeImageSelector(): void {
    this.isImageSeletorVisible = false;
  }
}
