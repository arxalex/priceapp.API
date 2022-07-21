Vue.component('categoryInsert', {
    template: `
        <div class="position-fixed window-insert">
            <div class="bg-white p-4 container shadow-lg rounded">
                <div class="d-flex mb-3"><h5 class="ms-1 fw-bold flex-fill">Category</h5>
                    <button class="btn mt-0 pt-0 px-0" @click="close">
                        <i class="bi bi-x text-danger"></i>
                    </button>
                </div>
                <div class="d-flex mb-3">
                    <div class="position-relative flex-fill me-2">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Id</label>
                        <input class="form-control" v-model="item.id" placeholder="Id">
                    </div>
                    <div class="position-relative flex-fill me-2">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Label</label>
                        <input class="form-control" v-model="item.label" placeholder="Label">
                    </div>
                    <div class="position-relative flex-fill me-2">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label z-5">Parent</label>
                        <selectSearch :elems="items" v-model="item.parent"></selectSearch>
                    </div>
                    <div class="position-relative mx-2 mt-2">
                        <label class="fw-light fs-min bg-white position-absolute input-label">isFilter</label>
                        <input class="form-check-input mx-2" type="checkbox" v-model="item.isFilter">
                    </div>
                </div>
                <h5 class="ms-1 fw-bold mb-3">Category link</h5>
                <div class="d-flex mb-3">
                    <div class="position-relative flex-fill me-2">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Id</label>
                        <input class="form-control" v-model="categoryLink.id" placeholder="Id">
                    </div>
                    <div class="position-relative flex-fill me-2">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label z-5">Category</label>
                        <selectSearch :elems="items" v-model="categoryLink.categoryid"></selectSearch>
                    </div>
                    <div class="position-relative flex-fill me-2">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label z-5">Shop</label>
                        <selectSearch :elems="shops" v-model="categoryLink.shopid"></selectSearch>
                    </div>
                    <div class="position-relative flex-fill me-2">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Category shop id</label>
                        <input class="form-control" v-model="categoryLink.categoryshopid">
                    </div>
                    <div class="position-relative flex-fill">
                        <label class="ms-2 px-1 fw-light bg-white position-absolute input-label">Shop category label</label>
                        <input class="form-control" v-model="categoryLink.shopcategorylabel">
                    </div>
                </div>
                <div class="input-group">
                    <button class="btn btn-secondary form-control" v-on:click='insertAndUpdate(1)'>Insert category</button>
                    <button class="btn btn-secondary form-control" v-on:click='insertAndUpdate(2)'>Update or insert link</button>
                    <button class="btn btn-primary form-control" v-on:click='insertAndUpdate(3)'>Insert and Update</button>
                </div>
            </div>
        </div>
    `,
    data() {
        return {
            item: this.sourceItem,
            categoryLink: {
                id: null,
                categoryid: null,
                shopid: null,
                categoryshopid: null,
                shopcategorylabel: null
            }
        }
    },
    props: {
        sourceItem: {
            type: Object
        },
    },
    methods: {
        insertAndUpdate: async function (variant) {
            const insertUrl = "../be/categories/insert_categories";
            const labelsUrl = "../be/items/get_labels";
            var data;
            switch (variant) {
                case 1:
                    data = {
                        method: "InsertToCategories",
                        category: this.item
                    }
                    break;
                case 2:
                    data = {
                        method: "InsertOrUpdateLink",
                        category_link: this.categoryLink
                    }
                    break;
                case 3:
                    data = {
                        method: "InsertToCategoriesAndUpdateLink",
                        category: this.item,
                        category_link: this.categoryLink
                    }
                    break;
            }
            await this.getItemsFromDb(insertUrl, data);
            
            var labels = await this.getItemsFromDb(labelsUrl, {
                method: "GetAllLabels"
            });
            Vue.prototype.$labels = labels;
            this.$labels = labels;
            this.$emit("itemInserted");
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
        close: async function(){
            const labelsUrl = "../be/items/get_labels";
            var labels = await this.getItemsFromDb(labelsUrl, {
                method: "GetAllLabels"
            });
            Vue.prototype.$labels = labels;
            this.$labels = labels;
            this.$emit("itemInserted");
        }
    },
    async mounted() {
        const cateoryLinkUrl = "../be/categories/get_categories";
        var categoryLink = await this.getItemsFromDb(cateoryLinkUrl, {
            method: "GetCategoryLinkByLabel",
            label: this.sourceItem.label
        });
        this.categoryLink = categoryLink;
    },
    computed: {
        items: {
            cache: false,
            get: function () {
                return this.$labels.categories;
            }
        },
        shops: function () {
            return this.$labels.shops;
        }
    }
});

