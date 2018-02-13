class FrightsController < ApplicationController
  before_action :set_fright, only: [:show, :edit, :update, :destroy]

  # GET /frights
  # GET /frights.json
  def index
    @frights = Fright.all
  end

  # GET /frights/1
  # GET /frights/1.json
  def show
  end

  # GET /frights/new
  def new
    @fright = Fright.new
  end

  # GET /frights/1/edit
  def edit
  end

  # POST /frights
  # POST /frights.json
  def create
    @fright = Fright.new(fright_params)

    respond_to do |format|
      if @fright.save
        format.html { redirect_to @fright, notice: 'Fright was successfully created.' }
        format.json { render :show, status: :created, location: @fright }
      else
        format.html { render :new }
        format.json { render json: @fright.errors, status: :unprocessable_entity }
      end
    end
  end

  # PATCH/PUT /frights/1
  # PATCH/PUT /frights/1.json
  def update
    respond_to do |format|
      if @fright.update(fright_params)
        format.html { redirect_to @fright, notice: 'Fright was successfully updated.' }
        format.json { render :show, status: :ok, location: @fright }
      else
        format.html { render :edit }
        format.json { render json: @fright.errors, status: :unprocessable_entity }
      end
    end
  end

  # DELETE /frights/1
  # DELETE /frights/1.json
  def destroy
    @fright.destroy
    respond_to do |format|
      format.html { redirect_to frights_url, notice: 'Fright was successfully destroyed.' }
      format.json { head :no_content }
    end
  end

  private
    # Use callbacks to share common setup or constraints between actions.
    def set_fright
      @fright = Fright.find(params[:id])
    end

    # Never trust parameters from the scary internet, only allow the white list through.
    def fright_params
      params.require(:fright).permit(:name)
    end
end
