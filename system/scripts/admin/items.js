Vue.component('Items', {
    template: `
        <div>
            <div class="position-fixed top-50 start-0 z-5 dropdown mx-3">
                <button class="btn btn-secondary " type="button" id="dropdownAuto" data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bi bi-sliders"></i>
                </button>
                <div class="dropdown-menu shadow-sm p-3 bg-body rounded" aria-labelledby="dropdownAuto" style="width: max-content;">
                    <div class="input-group">
                        <label class="fw-light fs-min bg-white input-label">Autoconvert to kg</label>
                        <input class="form-check-input mx-2" type="checkbox" v-model="helpers[0].isActive">
                    </div>
                    <div class="input-group">
                        <label class="fw-light fs-min bg-white input-label">Autopackage</label>
                        <select class="form-select ms-1 py-0" v-model="helpers[1].package">
                            <option v-for="package in packages" v-bind:value="package.id">{{ package.label }}</option>
                        </select>
                        <input class="form-check-input mx-2" type="checkbox" v-model="helpers[1].isActive">
                    </div>
                    <div class="input-group">
                        <label class="fw-light fs-min bg-white input-label">Autobrand insert</label>
                        <input class="form-check-input mx-2" type="checkbox" v-model="helpers[2].isActive">
                    </div>
                </div>
            </div>
            <itemsaver :sourceItem="itemsaverModel.sourceItem"
                :destinationItem="itemsaverModel.destinationItem"
                :originalLabels="itemsaverModel.originalLabels"
                :helpers="helpers"
                @insert="inserted(true)"
                @insertCanceled="inserted(false)"
                v-if="itemsaverModel.saveActive"></itemsaver>
            <div v-if="!itemsaverModel.saveActive && loaded">
                <h1>Prepare items</h1>
                <p>List:
                    <div v-for="shop in shops">
                        <span>{{ shop.id }}</span> - <span>{{ shop.label }}</span>
                    </div>
                </p>
                <div class="input-group mb-3">
                    <selectSearch :elems="shops" v-model="selectedShopId" style="flex: 1 1 auto"></selectSearch>
                    <selectSearch :elems="shopCategories" v-model="selectedCategoryId" style="flex: 1 1 auto"></selectSearch>
                </div>
                <div class="input-group mb-3">
                    <button @click="get_items()" class="btn btn-primary mt-2 w-100">Get items</button>
                </div>
                <div class="col-sm-12">
                    <item class="mb-3" 
                        v-for="itemModel in itemModels" 
                        :item="itemModel.item" 
                        :originalLabels="itemModel.originalLabels"
                        :similarItems="itemModel.similarItems"
                        v-on:itemsaver="itemsaverActivate">
                    </item>
                </div>
                <button class="btn btn-secondary" @click="get_prev()" :disabled="page.isPrevDisabled">Prev</button>
                <button class="btn btn-primary" @click="get_next()" :disabled="page.isNextDisabled">Next</button>
            </div>
        </div>
    `,
    data() {
        return {
            selectedShopId: null,
            selectedCategoryId: null,
            itemModels: [],
            shopCategories: [],
            loaded: false,
            page: {
                from: 0,
                to: 25,
                isPrevDisabled: true,
                isNextDisabled: false,
            },
            itemsaverModel: {
                sourceItem: null,
                destinationItem: null,
                saveActive: false,
                originalLabels: null
            },
            helpers: [
                {
                    isActive: true
                },
                {
                    isActive: true,
                    package: 5
                },
                {
                    isActive: true
                }
            ]
        }
    },
    methods: {
        get_categories: async function () {
            const categoriesUrl = "../be/categories/get_categories";
            var shop = this.selectedShopId;
            var data = {
                source: shop
            };
            var categories = await this.getItemsFromDb(categoriesUrl, data);
            this.shopCategories = categories;
        },
        get_items: async function () {
            this.itemModels = [];
            const similarUrl = "../be/items/get_similar_items";
            const getItemUrl = "../be/items/get_items";

            var shop = this.selectedShopId;
            var category = this.selectedCategoryId;

            var data = {
                source: shop,
                category: category,
                from: this.page.from,
                to: this.page.to
            };

            var items = await this.getItemsFromDb(getItemUrl, data);

            var itemLabels = [];
            items.forEach(element => {
                itemLabels.push(element.item.label);
            });
            var similarItems = await this.getItemsFromDb(similarUrl, {
                itemLabels: itemLabels
            });

            for (var i = 0; i < items.length; i++) {
                this.itemModels.push({
                    item: items[i].item,
                    originalLabels: items[i].originalLabels,
                    similarItems: similarItems[i]
                });
            }
        },
        get_prev: function () {
            if (this.page.from >= 25 && !this.page.isPrevDisabled) {
                this.isNextDisabled = false;
                this.page.from -= 25;
                this.page.to -= 25;
                this.get_items();
                if (this.page.from <= 0) {
                    this.page.isPrevDisabled = true;
                }
            }
        },
        get_next: function () {
            if (this.page.to <= 9975 && !this.page.isNextDisabled) {
                this.page.isPrevDisabled = false;
                this.page.from += 25;
                this.page.to += 25;
                this.get_items();
                if (this.page.to >= 10000) {
                    this.page.isNextDisabled = true;
                }
            }
        },
        getItemsFromDb: function (url, data) {
            return axios.post(url, data).then((response) => {
                if (response.status == 200) {
                    return response.data;
                }
            });
        },
        itemsaverActivate: function () {
            this.itemsaverModel = this.$itemsaver ?? {
                sourceItem: null,
                destinationItem: null,
                saveActive: false,
                originalLabels: null
            }
        },
        inserted: function (status) {
            if (status) {
                this.get_items();
            }
            this.itemsaverModel.saveActive = false;
        }
    },
    watch: {
        selectedShopId: function (value) {
            this.get_categories();
        }
    },
    async mounted() {
        const labelsUrl = "../be/items/get_labels";
        var labels = await this.getItemsFromDb(labelsUrl, {
            method: "GetAllLabels"
        });
        Vue.prototype.$labels = labels;
        console.log(1)
        this.loaded = true;
    },
    computed: {
        shops: function () {
            return this.$labels.shops;
        },
        packages: function () {
            return this.loaded ? this.$labels.packages : [];
        }
    }
});

